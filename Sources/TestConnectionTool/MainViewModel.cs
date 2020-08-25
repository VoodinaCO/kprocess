using KProcess.KL2.ConnectionSecurity;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TestConnectionTool.Models;
using WindowsFirewallHelper;

namespace TestConnectionTool
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public const string FirewallRuleName = @"Test Connection Tool KL²";

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public IDialogCoordinator DialogService { get; private set; }

        private static MainViewModel _instance = null;
        public static MainViewModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MainViewModel { DialogService = DialogCoordinator.Instance };
                return _instance;
            }
        }

        public static MetroWindow mainWindow { get; set; }

        public string UserSA => Const.UserSA;
        public string DefaultInstanceName => Const.InstanceName_v3;

        private string _TextLog = string.Empty;
        public string TextLog
        {
            get { return _TextLog; }
            set
            {
                if (_TextLog != value)
                {
                    _TextLog = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _NewInstance_UserName = null;
        public string NewInstance_UserName
        {
            get { return _NewInstance_UserName; }
            set
            {
                if (_NewInstance_UserName != value)
                {
                    _NewInstance_UserName = value;
                    RaisePropertyChanged();

                    RemoteInstanceTestOK = false;
                    dummyAddDataBase.KL2_DataBaseVersion = null;
                }
            }
        }

        private SecureString _NewInstance_Password = new SecureString();
        public SecureString NewInstance_Password
        {
            get { return _NewInstance_Password; }
            set
            {
                if (_NewInstance_Password != value)
                {
                    _NewInstance_Password = value;
                    RaisePropertyChanged();

                    RemoteInstanceTestOK = false;
                    dummyAddDataBase.KL2_DataBaseVersion = null;
                }
            }
        }

        public bool SQLInstancesScanned = false;

        private readonly static DataBase _dummyAddDataBase = new DataBase(true);
        public DataBase dummyAddDataBase => _dummyAddDataBase;

        private BindingList<DataBase> _dataBaseInstances = new BindingList<DataBase>();
        public BindingList<DataBase> DataBaseInstances
        {
            get
            {
                if (!_dataBaseInstances.Contains(_dummyAddDataBase))
                    _dataBaseInstances.Insert(0, _dummyAddDataBase);
                return _dataBaseInstances;
            }
            set
            {
                if (_dataBaseInstances != value)
                {
                    _dataBaseInstances = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool LocalDb12IsInstalled =>
            SQLServerDetection.LocalDB_IsInstalled();

        public bool HasNewLocalInstance =>
            SQLServerDetection.LocalDB_KL2Instance_Exists();

        private bool _hasOldLocalDatabase = false;
        public bool HasOldLocalDatabase
        {
            get { return _hasOldLocalDatabase; }
            set
            {
                if (_hasOldLocalDatabase != value)
                {
                    _hasOldLocalDatabase = value;
                    RaisePropertyChanged();
                }
            }
        }

        public DataBase OldLocalDatabase =>
            DataBaseInstances.DefaultIfEmpty(null).SingleOrDefault(_ => _.ServerName == Environment.MachineName && _.KL2_DataBaseVersion != null && _.KL2_DataBaseVersion < Version.Parse("3.0.0.0"));

        private bool _RemoteInstanceTestOK = false;
        public bool RemoteInstanceTestOK
        {
            get { return _RemoteInstanceTestOK; }
            private set
            {
                _RemoteInstanceTestOK = value;
                RaisePropertyChanged();
            }
        }

        private bool _PingTest = false;
        public bool PingTest
        {
            get { return _PingTest; }
            private set
            {
                _PingTest = value;
                RaisePropertyChanged();
            }
        }

        private bool _ConnectTest = false;
        public bool ConnectTest
        {
            get { return _ConnectTest; }
            private set
            {
                _ConnectTest = value;
                RaisePropertyChanged();
            }
        }

        private DataBase _selectedDataBaseInstance = null;
        public DataBase SelectedDataBaseInstance
        {
            get { return _selectedDataBaseInstance; }
            set
            {
                if (_selectedDataBaseInstance != value)
                {
                    Logger.ClearLog();
                    _selectedDataBaseInstance = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(DestinationInstance));

                    if (value == null || value == _dummyAddDataBase)
                    {
                        dummyAddDataBase.ServerName = null;
                        dummyAddDataBase.InstanceName = null;
                        dummyAddDataBase.KL2_DataBaseVersion = null;
                        RemoteInstanceTestOK = false;
                    }
                    else
                    {
                        dummyAddDataBase.ServerName = value.ServerName;
                        dummyAddDataBase.InstanceName = value.InstanceName;
                        dummyAddDataBase.KL2_DataBaseVersion = null;
                        if (_selectedDataBaseInstance.KL2_DataBaseVersion != null)
                            RemoteInstanceTestOK = true;
                        else
                            RemoteInstanceTestOK = false;
                    }

                    RaisePropertyChanged(nameof(SelectedInstanceIsDummy));
                    ((DelegateCommand<object>)TestDataBaseConnectionCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public DataBase DestinationInstance
        {
            get
            {
                if (SelectedDataBaseInstance == null)
                    return new DataBase { ServerName = "(LocalDb)", InstanceName = Const.InstanceName_v3 };
                else
                    return SelectedDataBaseInstance;
            }
        }

        public string DestinationInstanceName => $"{DestinationInstance.ServerName}\\{DestinationInstance.InstanceName}";

        public string DestinationDataBaseName => Const.DataBaseName_v3;

        public bool SelectedInstanceIsDummy =>
            SelectedDataBaseInstance == dummyAddDataBase;

        public void ExitApplication()
        {
            mainWindow.Close();
        }

        public bool OpenPortBeforeScanning()
        {
            try
            {
                var rule = WindowsFirewallHelper.FirewallAPIv2.Firewall.Instance.CreatePortRule(
                    FirewallManager.Instance.GetProfile().Type,
                    FirewallRuleName, FirewallAction.Allow, 80,
                    FirewallProtocol.UDP);
                FirewallManager.Instance.Rules.Add(rule);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ClosePortAfterScanning()
        {
            try
            {
                var myRules = FirewallManager.Instance.Rules.Where(r => r.Name == FirewallRuleName).ToArray();
                foreach (IRule myRule in myRules)
                    FirewallManager.Instance.Rules.Remove(myRule);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task ScanSQL_Instances(bool isCancelable = false)
        {
            if (!isCancelable && SQLInstancesScanned)
                return;
            var cancelTokenSource = new CancellationTokenSource();

            DataBaseInstances.Clear();

            OpenPortBeforeScanning();

            UdpClient udpClient = new UdpClient(80, AddressFamily.InterNetwork)
            {
                EnableBroadcast = true
            };

            var progressDialogController = await DialogService.ShowProgressAsync(this,
                "SQL Server instances searching",
                "Please wait...",
                isCancelable,
                new MetroDialogSettings
                {
                    NegativeButtonText = "Cancel",
                    CancellationToken = cancelTokenSource.Token
                });
            progressDialogController.SetIndeterminate();
            progressDialogController.Canceled += (o, e) =>
            {
                udpClient.Close();
                cancelTokenSource.Cancel();
            };

            // On utilise le service SQL Browser pour scanner les instances distantes
            cancelTokenSource.CancelAfter(5000);
            await udpClient.SendAsync(new byte[] { 0x02 }, 1, new IPEndPoint(IPAddress.Broadcast, 1434));
            while(!cancelTokenSource.IsCancellationRequested)
            {
                try
                {
                    var result = await udpClient.ReceiveAsync();
                    string[] instances = Encoding.ASCII.GetString(result.Buffer, 3, result.Buffer[1]).Split(new string[] { ";;" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string instance in instances)
                    {
                        var fields = instance.Split(';');
                        string serverName = fields?[1];
                        /*if (serverName == Environment.MachineName) // On ne répond pas au SQL Browser local
                            continue;*/
                        var dataBase = new DataBase
                        {
                            ServerName = serverName,
                            InstanceName = fields?[3],
                            Version = Version.Parse(fields?[7])
                        };
                        DataBaseInstances.Add(dataBase);
                    }
                }
                catch { break; }
            }

            //On teste les instances si ce sont des instances de KL2 avec les identifiants sa par défaut
            foreach (DataBase db in DataBaseInstances)
            {
                bool? newWithNewPass = await TestNewInstanceConnection(db, Const.UserSA, Const.PasswordSAEncrypted);
                if (newWithNewPass == true) // Identifiants bons et base v3 présente
                    continue;
                else if (newWithNewPass == false) // Identifiants bons, mais pas de base v3 présente
                    await TestOldInstanceConnection(db, Const.UserSA, Const.PasswordSAEncrypted);
                else // Identifiants incorrectes
                    await TestOldInstanceConnection(db, Const.UserSA, Const.OldPasswordSAEncrypted);
            }

            SQLInstancesScanned = true;
            ClosePortAfterScanning();
            RaisePropertyChanged(nameof(DataBaseInstances));
            RaisePropertyChanged(nameof(HasOldLocalDatabase));
            await progressDialogController.CloseAsync();
        }

        public ICommand TestDataBaseConnectionCommand { get; } = new DelegateCommand<object>(async e =>
        {
            Instance.RemoteInstanceTestOK = false;
            _dummyAddDataBase.KL2_DataBaseVersion = null;
            Logger.ClearLog();

            string[] serverName = _dummyAddDataBase.ServerName.Split(':');

            // Ping
            string machineName = serverName[0].Split('\\')[0];
            Logger.Write($"Ping {machineName} : ");
            Ping ping = new Ping();
            try
            {
                PingReply reply = await ping.SendPingAsync(machineName);
                Logger.WriteLine(reply.Status.ToString());
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"FAIL\nException :\n{ex.Message}");
                if (ex.InnerException != null)
                    Logger.WriteLine($"InnerException :\n{ex.InnerException.Message}");
                return;
            }

            // Test du port
            int port = 0;
            if (serverName.Length == 2 && int.TryParse(serverName.Last(), out port))
            {
                Logger.Write($"Port {port} test : ");
                TcpClient tcpClient = new TcpClient();
                try
                {
                    await tcpClient.ConnectAsync(machineName, port);
                    Logger.WriteLine(tcpClient.Connected ? "OK" : "FAIL");
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"FAIL\nException :\n{ex.Message}");
                    if (ex.InnerException != null)
                        Logger.WriteLine($"InnerException :\n{ex.InnerException.Message}");
                    return;
                }
            }

            // Test de connexion à l'instance
            Logger.WriteLine();
            Logger.WriteLine($"Connection to instance {_dummyAddDataBase.FullInstanceName} with");
            string dataSource = $"{serverName[0]}{(port == 0 ? string.Empty : $", {port}")}";
            Logger.WriteLine($"DataSource = {dataSource}");
            string userID = string.IsNullOrEmpty(Instance.NewInstance_UserName) ? Const.UserSA : Instance.NewInstance_UserName;
            Logger.WriteLine($"UserID = {userID}");
            string cryptedPassword = Instance.NewInstance_Password == null || Instance.NewInstance_Password.Length == 0 ? Const.PasswordSAEncrypted : ConnectionStringsSecurity.CryptPassword(Instance.NewInstance_Password);
            Logger.WriteLine($"Password = {(Instance.NewInstance_Password == null || Instance.NewInstance_Password.Length == 0 ? "Default KL² sa password" : "Filled password")}");

            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = dataSource,
                UserID = userID,
                Password = ConnectionStringsSecurity.DecryptPassword(cryptedPassword)
            };
            using (SqlConnection sqlConn = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                try
                {
                    await sqlConn.OpenAsync();
                    Logger.WriteLine($"Result : OK");
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"Exception :\n{ex.Message}");
                    if (ex.InnerException != null)
                        Logger.WriteLine($"Inner exception :\n{ex.InnerException.Message}");
                    return;
                }

                // Test présence base v3
                try
                {
                    SqlCommand sqlCommand = new SqlCommand($"USE [{Const.DataBaseName_v3}]", sqlConn);
                    await sqlCommand.ExecuteNonQueryAsync();
                    Logger.WriteLine($"Database v3 exists : YES");
                }
                catch
                {
                    Logger.WriteLine($"Database v3 exists : NO");
                    return;
                }

                // Test version database
                Logger.WriteLine($"Determine database version...");
                try
                {
                    SqlCommand sqlCommand = new SqlCommand($"USE [{Const.DataBaseName_v3}]", sqlConn);
                    await sqlCommand.ExecuteNonQueryAsync();
                    sqlCommand = new SqlCommand("SELECT [value] FROM SYS.EXTENDED_PROPERTIES WHERE class_desc='DATABASE' AND name='KL_Version'", sqlConn);
                    using (SqlDataReader result = await sqlCommand.ExecuteReaderAsync())
                    {
                        if (await result.ReadAsync())
                            Logger.WriteLine($"Database version : {Version.Parse(result.GetString(0))}");
                        else
                            Logger.WriteLine($"Database version : unknown");
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"Exception :\n{ex.Message}");
                    if (ex.InnerException != null)
                        Logger.WriteLine($"Inner exception :\n{ex.InnerException.Message}");
                    return;
                }
            }
        }, e => !(Instance.SelectedInstanceIsDummy && string.IsNullOrEmpty(_dummyAddDataBase.ServerName)) );

        public static async Task<bool?> TestOldInstanceConnection(DataBase dataBase, string user, string cryptedPassword)
        {
            if (dataBase == _dummyAddDataBase && !Instance.SQLInstancesScanned)
                return null;
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = $@"{(string.IsNullOrEmpty(dataBase.ServerName) ? Environment.MachineName : dataBase.ServerName)}\{(string.IsNullOrEmpty(dataBase.InstanceName) ? Const.InstanceName_v2 : dataBase.InstanceName)}",
                UserID = user,
                Password = ConnectionStringsSecurity.DecryptPassword(cryptedPassword)
            };
            bool? testResult = null;
            using (SqlConnection sqlConn = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                try
                {
                    await sqlConn.OpenAsync();
                }
                catch { return null; }
                try
                {
                    if (string.IsNullOrEmpty(dataBase.ServerName))
                        dataBase.ServerName = Environment.MachineName;
                    if (string.IsNullOrEmpty(dataBase.InstanceName))
                        dataBase.InstanceName = Const.InstanceName_v2;
                    dataBase.UserSA = user;
                    dataBase.CryptedPasswordSA = cryptedPassword;

                    SqlCommand sqlCommand = new SqlCommand($"USE [{Const.DataBaseName_v2}]", sqlConn);
                    await sqlCommand.ExecuteNonQueryAsync();
                    sqlCommand = new SqlCommand("SELECT [value] FROM SYS.EXTENDED_PROPERTIES WHERE class_desc='DATABASE' AND name='KL_Version'", sqlConn);
                    using (SqlDataReader result = await sqlCommand.ExecuteReaderAsync())
                    {
                        if (await result.ReadAsync())
                            dataBase.KL2_DataBaseVersion = Version.Parse(result.GetString(0));
                    }
                    testResult = true;
                }
                catch { testResult = false; }
                finally { sqlConn.Close(); }
            }
            return testResult;
        }

        public static async Task<bool?> TestNewInstanceConnection(DataBase dataBase, string user, string cryptedPassword)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = $@"{(string.IsNullOrEmpty(dataBase.ServerName) ? Environment.MachineName : dataBase.ServerName)}\{(string.IsNullOrEmpty(dataBase.InstanceName) ? Const.InstanceName_v3 : dataBase.InstanceName)}",
                UserID = user,
                Password = ConnectionStringsSecurity.DecryptPassword(cryptedPassword)
            };
            bool? testResult = null;
            using (SqlConnection sqlConn = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                try
                {
                    await sqlConn.OpenAsync();
                }
                catch { return null; }
                try
                {
                    if (string.IsNullOrEmpty(dataBase.ServerName))
                        dataBase.ServerName = Environment.MachineName;
                    if (string.IsNullOrEmpty(dataBase.InstanceName))
                        dataBase.InstanceName = Const.InstanceName_v3;
                    dataBase.UserSA = user;
                    dataBase.CryptedPasswordSA = cryptedPassword;

                    SqlCommand sqlCommand = new SqlCommand($"USE [{Const.DataBaseName_v3}]", sqlConn);
                    await sqlCommand.ExecuteNonQueryAsync();
                    sqlCommand = new SqlCommand("SELECT [value] FROM SYS.EXTENDED_PROPERTIES WHERE class_desc='DATABASE' AND name='KL_Version'", sqlConn);
                    using (SqlDataReader result = await sqlCommand.ExecuteReaderAsync())
                    {
                        if (await result.ReadAsync())
                            dataBase.KL2_DataBaseVersion = Version.Parse(result.GetString(0));
                    }
                    testResult = true;
                }
                catch { testResult = false; }
                finally { sqlConn.Close(); }
            }
            return testResult;
        }

        public static async Task<bool?> TestConnection(DataBase dataBase, string user, string cryptedPassword, string dbName = Const.DataBaseName_v3)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = $@"{dataBase.ServerName}\{dataBase.InstanceName}",
                UserID = user,
                Password = ConnectionStringsSecurity.DecryptPassword(cryptedPassword)
            };
            bool? testResult = null;
            using (SqlConnection sqlConn = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                try
                {
                    await sqlConn.OpenAsync();
                }
                catch { return null; }
                try
                {
                    dataBase.UserSA = user;
                    dataBase.CryptedPasswordSA = cryptedPassword;

                    SqlCommand sqlCommand = new SqlCommand($"USE [{dbName}]", sqlConn);
                    await sqlCommand.ExecuteNonQueryAsync();
                    sqlCommand = new SqlCommand("SELECT [value] FROM SYS.EXTENDED_PROPERTIES WHERE class_desc='DATABASE' AND name='KL_Version'", sqlConn);
                    using (SqlDataReader result = await sqlCommand.ExecuteReaderAsync())
                    {
                        if (await result.ReadAsync())
                            dataBase.KL2_DataBaseVersion = Version.Parse(result.GetString(0));
                    }
                    testResult = true;
                }
                catch { testResult = false; }
                finally { sqlConn.Close(); }
            }
            return testResult;
        }
    }
}
