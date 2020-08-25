using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran A propos.
    /// </summary>
    class AboutViewModel : ViewModelBase, IAboutViewModel
    {

        #region Champs privés

        #endregion

        #region Surcharges

        /// <summary>
        /// Obtient le titre
        /// </summary>
        public override string Title
        {
            get { return LocalizationManagerExt.GetSafeDesignerString("View_AboutView_Title"); }
        }

        /// <summary>
        /// Initialise le VM en vue du chargement
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// Méthode appelée lors du chargement
        /// </summary>
        protected override Task OnLoading() =>
            Task.CompletedTask;

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override Task OnInitializeDesigner() =>
            Task.CompletedTask;

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient la version.
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// Obtient l'adresse de contact.
        /// </summary>
        public string Contact
        {
            get
            {
                return PresentationConstants.ContactEmail;
            }
        }

        /// <summary>
        /// Obtient l'adresse du site web.
        /// </summary>
        public string Website
        {
            get
            {
                return PresentationConstants.Website;
            }
        }

        /// <summary>
        /// Obtient le numéro IDDN.
        /// </summary>
        public string IDDNNumber
        {
            get
            {
                return PresentationConstants.IDDN;
            }
        }

        #endregion

        #region Commandes

        private Command _contactCommand;
        public ICommand ContactCommand
        {
            get
            {
                if (_contactCommand == null)
                    _contactCommand = new Command(() =>
                    {
                        try
                        {
                            Process.Start(new Maito() { To = new string[] { Contact } }.ToString());
                        }
                        catch { }
                    });
                return _contactCommand;
            }
        }

        private Command _websiteCommand;
        public ICommand WebsiteCommand
        {
            get
            {
                if (_websiteCommand == null)
                    _websiteCommand = new Command(() =>
                {
                    try
                    {
                        Process.Start(Website);
                    }
                    catch { }
                });
                return _websiteCommand;
            }
        }

        #endregion

    }
}
