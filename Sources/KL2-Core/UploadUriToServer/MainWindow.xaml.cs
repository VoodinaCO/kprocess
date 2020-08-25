using KProcess.KL2.ConnectionSecurity;
using Murmur;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace UploadUriToServer
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisedPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        const string DataSourceKey = "DataSource";
        const string InitialCatalogKey = "InitialCatalog";

        public MainWindow()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                DataContext = this;
            };
        }

        BindingList<ResourceFile> _resourceFiles = new BindingList<ResourceFile>();
        public BindingList<ResourceFile> ResourceFiles
        {
            get => _resourceFiles;
            set
            {
                if (_resourceFiles != value)
                {
                    _resourceFiles = value;
                    RaisedPropertyChanged();
                }
            }
        }

        List<string> cloudFiles = new List<string>();

        string _status;
        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    RaisedPropertyChanged();
                }
            }
        }

        DelegateCommand _extractBinCommand;
        public ICommand ExtractBinCommand
        {
            get
            {
                if (_extractBinCommand == null)
                {
                    _extractBinCommand = new DelegateCommand(async () =>
                    {
                        if (!ConfigurationManager.AppSettings.AllKeys.Contains(DataSourceKey))
                        {
                            MessageBox.Show($"Veuillez renseigner la clé {DataSourceKey} dans le fichier config.");
                            return;
                        }
                        if (!ConfigurationManager.AppSettings.AllKeys.Contains(InitialCatalogKey))
                        {
                            MessageBox.Show($"Veuillez renseigner la clé {InitialCatalogKey} dans le fichier config.");
                            return;
                        }

                        ResourceFiles = new BindingList<ResourceFile>();

                        // We create the temp folder
                        string extractDirectory = Path.Combine(Path.GetTempPath(), "KL2_Extract");
                        if (Directory.Exists(extractDirectory))
                        {
                            try
                            {
                                Directory.Delete(extractDirectory, true);
                                while (Directory.Exists(extractDirectory))
                                    await Task.Delay(TimeSpan.FromMilliseconds(200));
                            }
                            catch
                            {
                            }
                        }
                        Directory.CreateDirectory(extractDirectory);

                        try
                        {
                            using (var sqlConn = new SqlConnection(GetConnectionString()))
                            {

                                async Task ReadUriFromDataBase(string tableName, string idName)
                                {
                                    SqlDataReader reader = await ExecuteCommandFormat<SqlDataReader>(sqlConn, $"SELECT [{idName}],[{ResourceFile.UriPropertyName}] FROM [dbo].[{tableName}] WHERE [{ResourceFile.UriPropertyName}] IS NOT NULL;");
                                    if (reader.HasRows)
                                        while (reader.Read())
                                        {
                                            var id = reader.GetInt32(0);
                                            var uri = reader.GetString(1);
                                            ResourceFiles.Add(new ResourceFile
                                            {
                                                TableName = tableName,
                                                IdPropertyName = idName,
                                                Id = id,
                                                Uri = uri
                                            });
                                        }
                                    reader.Close();
                                    sqlConn.Close();
                                }

                                async Task ReadCloudFileFromDataBase()
                                {
                                    SqlDataReader reader = await ExecuteCommandFormat<SqlDataReader>(sqlConn, $"SELECT [{ResourceFile.HashPropertyName}] FROM [dbo].[CloudFile];");
                                    if (reader.HasRows)
                                        while (reader.Read())
                                            cloudFiles.Add(reader.GetString(0));
                                    reader.Close();
                                    sqlConn.Close();
                                }

                                Status = "List CloudFiles from database";
                                await ReadCloudFileFromDataBase();

                                Status = "List files from database";
                                await ReadUriFromDataBase("RefResource", "ResourceId");
                                await ReadUriFromDataBase("RefActionCategory", "ActionCategoryId");
                                await ReadUriFromDataBase("Ref1", "RefId");
                                await ReadUriFromDataBase("Ref2", "RefId");
                                await ReadUriFromDataBase("Ref3", "RefId");
                                await ReadUriFromDataBase("Ref4", "RefId");
                                await ReadUriFromDataBase("Ref5", "RefId");
                                await ReadUriFromDataBase("Ref6", "RefId");
                                await ReadUriFromDataBase("Ref7", "RefId");

                                Status = "Test if files exist";
                                foreach (var file in ResourceFiles)
                                {
                                    if (File.Exists(file.Uri))
                                        file.LocalFileExists = true;
                                }

                                Status = "Compute hashes";
                                foreach (var file in ResourceFiles.Where(_ => _.LocalFileExists))
                                {
                                    HashAlgorithm murmur128 = MurmurHash.Create128(managed: false);
                                    using (var fileStream = File.OpenRead(file.Uri))
                                    {
                                        file.Hash = ToHashString(murmur128.ComputeHash(fileStream));
                                    }
                                }

                                Status = "Copy files with hash name";
                                foreach (var file in ResourceFiles.Where(_ => _.LocalFileExists && _.Hash != null))
                                {
                                    string dstFilePath = Path.Combine(extractDirectory, $"{file.Hash}{Path.GetExtension(file.Uri)}");
                                    if (!File.Exists(dstFilePath))
                                        File.Copy(file.Uri, dstFilePath);
                                    file.Copied = true;
                                }

                                Status = "Add new file to CloudFile table";
                                foreach (var file in ResourceFiles.Where(_ => _.Copied))
                                {
                                    if (cloudFiles.Contains(file.Hash))
                                        file.AlreadyInBase = true;
                                    else
                                    {
                                        await ExecuteCommandFormat<int>(sqlConn, "INSERT INTO [dbo].[CloudFile] ([Hash],[Extension]) VALUES ('{0}',{1})", file.Hash, string.IsNullOrEmpty(Path.GetExtension(file.Uri)) ? "NULL" : $"'{Path.GetExtension(file.Uri)}'");
                                        cloudFiles.Add(file.Hash);
                                    }
                                }

                                Status = "Update Hash properties";
                                foreach (var file in ResourceFiles.Where(_ => _.Hash != null))
                                    await ExecuteCommandFormat<int>(sqlConn, $"UPDATE [dbo].[{file.TableName}] SET [{ResourceFile.HashPropertyName}] = '{file.Hash}' WHERE [{file.IdPropertyName}] = {file.Id};");
                            }
                            MessageBox.Show($"{ResourceFiles.Count(_ => !_.LocalFileExists)} file(s) not found.\n" +
                                $"{ResourceFiles.Count(_ => _.Copied)} file(s) copied to {extractDirectory}.\n" +
                                $"{ResourceFiles.Count(_ => !_.AlreadyInBase)} cloud file(s) added to database.");
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message, "An error has been raised");
                        }
                    });
                    RaisedPropertyChanged();
                }
                return _extractBinCommand;
            }
        }

        public static string GetConnectionString()
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = ConfigurationManager.AppSettings[DataSourceKey],
                InitialCatalog = ConfigurationManager.AppSettings[InitialCatalogKey],
                UserID = Const.DataBaseAdminUser,
                Password = ConnectionStringsSecurity.DecryptPassword(Const.DataBaseAdminCryptedPassword)
            };
            return connectionStringBuilder.ConnectionString;
        }

        public static void ExecuteScript(SqlConnection conn, string scriptName, bool useDatabase = false, string databaseName = Const.DataBaseName_v3)
        {
            try
            {
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"MigrationBinaries.Scripts.{scriptName}.sql");
                string script;
                using (StreamReader reader = new StreamReader(stream))
                    script = reader.ReadToEnd();

                conn.Open();
                SqlCommand cmd = new SqlCommand($"USE [{databaseName}];", conn);
                if (useDatabase)
                    cmd.ExecuteNonQuery();
                foreach (var query in script.Split(new[] { "\nGO" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    try
                    {
                        cmd.CommandText = query;
                        int result = cmd.ExecuteNonQuery();
                    }
                    catch (Exception inner)
                    {
                        throw new Exception($"SQL ERROR - {inner.Message}\n{query}", inner);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                try { conn.Close(); } catch { }
            }
        }

        public static async Task<T> ExecuteCommandFormat<T>(SqlConnection conn, string command, params object[] args)
        {
            try
            {
                T result;
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand(string.Format(command, args), conn);
                if (typeof(T) == typeof(int))
                {
                    result = (T)(object)await cmd.ExecuteNonQueryAsync();
                    conn.Close();
                }
                else if (typeof(T) == typeof(SqlDataReader))
                {
                    result = (T)(object)await cmd.ExecuteReaderAsync();
                }
                else if (typeof(T) == typeof(XmlReader))
                {
                    result = (T)(object)await cmd.ExecuteXmlReaderAsync();
                }
                else
                {
                    result = (T)await cmd.ExecuteScalarAsync();
                    conn.Close();
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string ToHashString(byte[] hash)
        {
            var builder = new StringBuilder();
            foreach (byte hashed in hash)
                builder.AppendFormat("{0:X2}", hashed);
            return builder.ToString();
        }
    }
}
