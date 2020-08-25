using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace KProcess.Ksmed.Security.Activation.Providers
{
    public class UserInformationProvider : IUserInformationProvider
    {
        private static object _syncRoot = new object();

        [JsonProperty]
        public string Username { get; private set; }

        [JsonProperty]
        public string Company { get; private set; }

        [JsonProperty]
        public string Email { get; private set; }

        public UserInformationProvider(bool refresh = false)
        {
            if (refresh)
                Refresh();
        }

        public void Refresh()
        {
            var machineStore = IsolatedStorageFile.GetMachineStoreForAssembly();
            if (machineStore != null)
            {
                if (machineStore.FileExists("UserInfo"))
                {
                    IsolatedStorageFileStream stream = new IsolatedStorageFileStream("UserInfo"
                        , System.IO.FileMode.Open, FileAccess.Read, FileShare.ReadWrite, machineStore);

                    try
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(UserInformation));
                            var user = (UserInformation)serializer.Deserialize(reader);
                            this.Username = user.Username;
                            this.Company = user.Company;
                            this.Email = user.Email;
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        public void SetUserInformation(string username, string company, string email)
        {
            lock (_syncRoot)
            {
                var machineStore = IsolatedStorageFile.GetMachineStoreForAssembly();
                if (machineStore != null)
                {
                    IsolatedStorageFileStream stream = new IsolatedStorageFileStream("UserInfo"
                        , System.IO.FileMode.Create, machineStore);

                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        var userInfo = new UserInformation { Username = username, Company = company, Email = email };
                        XmlSerializer serializer = new XmlSerializer(typeof(UserInformation));
                        serializer.Serialize(writer, userInfo);
                    }
                }
            }

            Refresh();
        }

        [DataContract]
        public class UserInformation
        {
            [DataMember]
            public string Username { get; set; }

            [DataMember]
            public string Company { get; set; }

            [DataMember]
            public string Email { get; set; }
        }
    }
}
