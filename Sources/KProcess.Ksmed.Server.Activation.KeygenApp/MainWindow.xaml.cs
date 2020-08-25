using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using KProcess.Common;
using KProcess.Ksmed.Security.Activation;
using Microsoft.Win32;
using System.Threading.Tasks;
using KProcess.Ksmed.Server.Activation.KeygenApp.Freshdesk.Models;
using System.Text.RegularExpressions;
using System.Reflection;

namespace KProcess.Ksmed.Server.Activation.KeygenApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FreshdeskApiConnector conn = new FreshdeskApiConnector();
        Company selectedCompany = null;
        User selectedUser = null;

        public BulkObservableCollection<User> MailSelectedUsers;


        public MainWindow()
        {
            InitializeComponent();

            this.Features = new WrapperFeatures[]
            {
                new WrapperFeatures { Text = "Toutes", Value = (short)ActivationFeatures.All },
                new WrapperFeatures { Text = "Lecture", Value = (short)ActivationFeatures.ReadOnly },
            };

            this.DataContext = this;
        }
        public WrapperFeatures[] Features { get; private set; }

        private async Task RefreshClientsAsync()
        {
            cbClient.ItemsSource = null;
            lb_clients.ItemsSource = null;

            //Ne récupère que les endUsers
            try
            {
                List<UserExt> listUserExt = new List<UserExt>();
                List<CompanyExt> listCompany = new List<CompanyExt>();

                var clients = (await conn.GetCustomerListAsync()).Where(x => !x.Other.EndUserLink.IsNotNullNorEmpty());
                var companies = await conn.GetCompanyListAsync();
                cbClient.ItemsSource = clients;

                foreach (var u in clients)
                {                   
                    if (companies.Exists(c => c.Id == u.CompanyId))
                    {
                        listUserExt.Add(new UserExt(u, companies.SingleOrDefault(c => c.Id == u.CompanyId)));
                    }
                    else
                        listUserExt.Add(new UserExt(u, new Company { Name = "" }));
                }

               

                lb_clients.ItemsSource = listUserExt;

                foreach (var c in companies)
                {
                    if (clients.ToList().Exists(client => client.CompanyId == c.Id && client.Other.Updated==true) )
                    {
                        listCompany.Add(new CompanyExt { Company = c, Users = clients.ToList().Where(client => client.CompanyId == c.Id && client.Other.Updated == true).ToList() });
                    }
                }
                lb_societe.ItemsSource = listCompany;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + Environment.NewLine + "Inner exception:" + e.InnerException.Message);
            }
        }

        private async Task RefreshInfosAsync()
        {
            this.tbClientID.Foreground = new SolidColorBrush(Colors.Black);

            if (cbClient.SelectedItem != null && cbClient.SelectedItem is User)
            {
                selectedUser = ((User)cbClient.SelectedItem);

                this.tbTrialDays.Text = selectedUser.Other.LicenceDuration.ToString();
                this.tbClientID.Text = selectedUser.Other.CustomerId;
                this.tbUsername.Text = selectedUser.Other.LicenceKeyName;
                this.tbCompany.Text = selectedUser.Other.LicenceKeyCompany;
                this.tbEmail.Text = selectedUser.Other.LicenceKeyMail;
                this.tbMachinehash.Text = selectedUser.Other.LicenceKeyMachineId;

                if (selectedUser.CompanyId.HasValue)
                {
                    selectedCompany = await conn.GetCompanyByIdAsync(selectedUser.CompanyId.Value.ToString());
                    if (!selectedUser.Other.CustomerId.IsNotNullNorEmpty())
                    {
                        string licenceKeyIdentifier = selectedCompany.GetNextLicenceKeyIdentifier();

                        if (licenceKeyIdentifier.IsNotNullNorEmpty())
                            this.tbClientID.Text = licenceKeyIdentifier;
                        else
                        {
                            SetIdClientError("Identifiant société invalide ou inexistant.");
                        }
                    }
                }
                else
                {
                    SetIdClientError("Aucune société liée.");
                }
            }
        }

        private void SetIdClientError(string error)
        {
            this.tbClientID.Text = error;
            this.tbClientID.Foreground = new SolidColorBrush(Colors.Red);
        }


        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            short trialDays;
            if (!short.TryParse(tbTrialDays.Text, out trialDays) && trialDays >= 0)
            {
                ShowError("Jours d'essai", string.Format("Nombre entre 0 et {0}", short.MaxValue));
                return;
            }

            string clientID = this.tbClientID.Text.Trim();
            if (clientID.Length > 6 || clientID.Length == 0)
            {
                ShowError("Identifiant client invalide", "L'identifiant client est composé de l'identifiant de la société récupéré dans freshdesk (3 ou 4 caractères) + numéro de licence généré automatiquement");
                return;
            }

            string username = this.tbUsername.Text.Trim();
            string company = this.tbCompany.Text.Trim();
            string email = this.tbEmail.Text.Trim();
            string machineHash = this.tbMachinehash.Text.Trim();

            ProductLicenseInfo licenseInfo;

            try
            {
                licenseInfo = new ProductKeyService().GenerateAndActivateProductKey(
                    ActivationConstants.ProductId,
                    ((WrapperFeatures)this.comboFeatures.SelectedItem).Value,
                    trialDays,
                    clientID,
                    username,
                    company,
                    email,
                    machineHash
                    );

                if (licenseInfo == null || licenseInfo.ActivationInfo == null || licenseInfo.Signature == null)
                {
                    MessageBox.Show("La génération de la clé a échoué");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            var sfd = new SaveFileDialog()
            {
                AddExtension = true,
                DefaultExt = ActivationConstants.DefaultKeyExtension,
                RestoreDirectory = true,
                FileName = IOHelper.ReplaceInvalidFileNameChars(clientID),
                Filter = ActivationConstants.DefaultKeyFilter,
            };

            if (sfd.ShowDialog().GetValueOrDefault())
            {
                string filepath = sfd.FileName;

                try
                {
                    using (StreamWriter writer = new StreamWriter(filepath, false))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(ProductLicenseInfo));
                        serializer.Serialize(writer, licenseInfo);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

                if (selectedCompany != null && selectedUser != null)
                {
                    selectedUser.Other.LicenceDuration = int.Parse(tbTrialDays.Text.Trim());
                    selectedUser.Other.CustomerId = tbClientID.Text.Trim();
                    selectedUser.Other.LicenceDateRenewal = DateTime.Now;

                    if (selectedCompany.Other.MaintenanceAnniversaryDate == null)
                        selectedCompany.Other.MaintenanceAnniversaryDate = DateTime.Now.AddMonths(6);

                    if (!selectedUser.Other.FirstInstallDate.HasValue)
                        selectedUser.Other.FirstInstallDate = DateTime.Now;

                    conn.UpdateUser(selectedUser);
                    conn.UpdateCompany(selectedCompany);
                }
            }
        }

        private void ShowError(string field, string reason)
        {
            MessageBox.Show(string.Format("Le champ {0} est invalide : {1}", field, reason));
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            this.tbTrialDays.Text = string.Empty;
            this.tbClientID.Text = string.Empty;
            this.tbUsername.Text = string.Empty;
            this.tbCompany.Text = string.Empty;
            this.tbEmail.Text = string.Empty;
            this.tbMachinehash.Text = string.Empty;
        }

        private async void cbClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await RefreshInfosAsync();
        }

        private async void btRefreshCustomers_Click(object sender, RoutedEventArgs e)
        {
            var oldSelectedItem = cbClient.SelectedItem;

            await RefreshClientsAsync();

            if (oldSelectedItem != null
                && cbClient.Items.Contains(oldSelectedItem))
            {
                cbClient.SelectedIndex = cbClient.Items.IndexOf(oldSelectedItem);
                await RefreshInfosAsync();
            }
        }

        private void tbTrialDays_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void tbTrialDays_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await RefreshClientsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible de charger les clients: " + ex.Message);
            }
        }

        private void btnGenerateMail_Click(object sender, RoutedEventArgs e)
        {
            bool retValue = true;
            List<string> unsentUsers = new List<string>();
            if (MessageBox.Show(
                   $"Etes-vous sûrs de vouloir envoyer le mail d'installation de Kl² aux {lb_clients.SelectedItems.Count} utilisateurs sélectionnés ?",
                   "Envoi du mail d'installation de Kl²®",
                   MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                foreach (UserExt userSelected in lb_clients.SelectedItems)
                {


                    OutboundMail mail = new Freshdesk.Models.OutboundMail();

                    //mail.RequesterName = userSelected.Name;
                    mail.RequesterEmail = userSelected.User.EmailAddress;
                    mail.Status = 5;
                    mail.Priority = 1;
                    mail.EmailConfigId = 16000018357;
                    mail.Type = "Licence";
                    mail.Tags = new List<string> { "KL²" };

                    //mail.Attachments = new List<byte[]>() { Properties.Resources.Lisez_moi };



                    mail.Subject = userSelected.User.Other.Language == "Français" ? "KL²® - Comment installer votre licence" : "KL²® - How to install your software";

                    string dlLink = string.Format("http://install.k-process.com/Kl2.zip?{0}", Uri.EscapeDataString(userSelected.User.Name));

                    mail.HtmlContent = string.Format(
                        userSelected.User.Other.Language == "Français" ? Properties.Resources.mail_kl2_fr : Properties.Resources.mail_kl2_en,
                        userSelected.User.Name,
                        dlLink
                        );



                    retValue &= conn.CreateOutboundMail(mail);
                    if (retValue)
                    {
                        userSelected.User.Other.VersionKL2 = tbxVersion.Text;
                        conn.UpdateUser(userSelected.User);
                    }
                    else
                    {
                        unsentUsers.Add(userSelected.User.Name);
                    }

                }

                if (retValue)
                {
                    MessageBox.Show("Le(s) mail(s) a(ont) bien été envoyé(s).", "Envoi Réussi!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    string errorContent = "Le(s) mail(s) n'a(ont) pas pu être envoyé(s) aux clients suivants: " + Environment.NewLine + Environment.NewLine;
                    foreach (var user in unsentUsers)
                    {
                        errorContent += $"- {user + Environment.NewLine}";
                    }
                    MessageBox.Show(errorContent, "Envoi impossible", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnGenerateUpdateMail_Click(object sender, RoutedEventArgs e)
        {
            bool retValue = true;
            List<string> unsentUsers = new List<string>();

            if (MessageBox.Show(
                   $"Etes-vous sûrs de vouloir envoyer le mail de mise à jour vers la dernière version de Kl² aux {lb_clients.SelectedItems.Count} utilisateurs sélectionnés ?",
                   "Envoi du mail de mise à jour de Kl²®",
                   MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                foreach (UserExt userSelected in lb_clients.SelectedItems)
                {


                    OutboundMail mail = new Freshdesk.Models.OutboundMail();

                    //mail.RequesterName = userSelected.Name;
                    mail.RequesterEmail = userSelected.User.EmailAddress;
                    mail.Status = 5;
                    mail.Priority = 1;
                    mail.EmailConfigId = 16000018357;
                    mail.Type = "Licence";
                    mail.Tags = new List<string> { "KL²" };

                    //mail.Attachments = new List<byte[]>() { Properties.Resources.Lisez_moi };



                    mail.Subject = userSelected.User.Other.Language == "Français" ? "KL²® - Comment mettre à jour votre licence" : "KL²® - How to update your software";

                    string dlLink = string.Format("http://install.k-process.com/Kl2.zip?{0}", Uri.EscapeDataString(userSelected.User.Name));

                    mail.HtmlContent = string.Format(
                        userSelected.User.Other.Language == "Français" ? Properties.Resources.mail_update_kl2_fr : Properties.Resources.mail_update_kl2_en,
                        userSelected.User.Name,
                        dlLink
                        );

                    retValue &= conn.CreateOutboundMail(mail);
                    if (retValue)
                    {
                        userSelected.User.Other.VersionKL2 = tbxVersion.Text;
                        conn.UpdateUser(userSelected.User);
                    }
                    else
                    {
                        unsentUsers.Add(userSelected.User.Name);
                    }
                }

                if (retValue)
                {
                    MessageBox.Show("Le(s) mail(s) a(ont) bien été envoyé(s).", "Envoi Réussi!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    string errorContent = "Le(s) mail(s) n'a(ont) pas pu être envoyé(s) aux clients suivants: " + Environment.NewLine + Environment.NewLine;
                    foreach (var user in unsentUsers)
                    {
                        errorContent += $"- {user + Environment.NewLine}";
                    }
                    MessageBox.Show(errorContent, "Envoi impossible", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnGenerateUpdateMailCompany_Click(object sender, RoutedEventArgs e)
        {
            bool retValue = true;
            List<string> unsentUsers = new List<string>();

            if (MessageBox.Show(
                   $"Etes-vous sûrs de vouloir envoyer le mail de mise à jour vers la dernière version de Kl² aux {lb_clients.SelectedItems.Count} sociétés sélectionnées ?",
                   "Envoi du mail de mise à jour de Kl²®",
                   MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                foreach (CompanyExt companySelected in lb_societe.SelectedItems)
                {
                    foreach (User userSelected in companySelected.Users.Where(u=> new Version(u.Other.VersionKL2)<=new Version(tbxVersion.Text)))
                    {

                        OutboundMail mail = new Freshdesk.Models.OutboundMail();

                        //mail.RequesterName = userSelected.Name;
                        mail.RequesterEmail = userSelected.EmailAddress;
                        mail.Status = 5;
                        mail.Priority = 1;
                        mail.EmailConfigId = 16000018357;
                        mail.Type = "Licence";
                        mail.Tags = new List<string> { "KL²" };

                        //mail.Attachments = new List<byte[]>() { Properties.Resources.Lisez_moi };



                        mail.Subject = userSelected.Other.Language == "Français" ? "KL²® - Comment mettre à jour votre licence" : "KL²® - How to update your software";

                        string dlLink = string.Format("http://install.k-process.com/Kl2.zip?{0}", Uri.EscapeDataString(userSelected.Name));

                        mail.HtmlContent = string.Format(
                            userSelected.Other.Language == "Français" ? Properties.Resources.mail_update_kl2_fr : Properties.Resources.mail_update_kl2_en,
                            userSelected.Name,
                            dlLink
                            );

                        retValue &= conn.CreateOutboundMail(mail);
                        if (retValue)
                        {
                           
                            userSelected.Other.VersionKL2 = tbxVersion.Text;
                            
                            conn.UpdateUser(userSelected);
                         }
                        else
                        {
                            unsentUsers.Add($"{companySelected.Company.Name} : {userSelected.Name}");
                        }                      
                    }

                    //On met à jour la version dans la société
                    companySelected.Company.Other.VersionKL2 = tbxVersion.Text;
                    conn.UpdateCompany(companySelected.Company);
                }

                if (retValue)
                {
                    MessageBox.Show("Le(s) mail(s) a(ont) bien été envoyé(s).", "Envoi Réussi!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    string errorContent = "Le(s) mail(s) n'a(ont) pas pu être envoyé(s) aux clients suivants: " + Environment.NewLine + Environment.NewLine;
                    foreach (var user in unsentUsers)
                    {
                        errorContent += $"- {user + Environment.NewLine}";
                    }
                    MessageBox.Show(errorContent, "Envoi impossible", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void lb_societe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count>0 && e.AddedItems[0]!=null && e.AddedItems[0] is CompanyExt)
            {
                lb_userPerCompany.ItemsSource = ((CompanyExt)e.AddedItems[0]).Users;
            }
        }
    }

    public class UserExt
    {
        public Company Company { get; set; }
        public User User { get; set; }
        public DateTime? LicenseExpirationDate
        {
            get
            {
                if (this.User.Other.LicenceDateRenewal.HasValue && this.User.Other.LicenceDuration.HasValue)
                    return this.User.Other.LicenceDateRenewal.Value.AddDays(this.User.Other.LicenceDuration.Value);

                return null;
            }
        }

        public UserExt()
        {

        }

        public UserExt(User user, Company company)
        {
            this.Company = company;
            this.User = user;
        }


    }

    public class CompanyExt
    {
        public Company Company { get; set; }
        public List<User> Users { get; set; }


        public CompanyExt()
        {

        }
    }
}
