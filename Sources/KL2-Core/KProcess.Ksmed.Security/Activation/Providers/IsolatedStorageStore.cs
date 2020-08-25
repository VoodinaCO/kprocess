using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

namespace KProcess.Ksmed.Security.Activation
{
    class IsolatedStorageStore : ILicenseStore
    {
        private static object _syncRoot = new object();

        public ProductLicenseInfo LoadLicense(string productName)
        {
            using (var machineStore = IsolatedStorageFile.GetMachineStoreForAssembly())
            {
                if (machineStore != null)
                {
                    if (machineStore.FileExists(productName))
                    {
                        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(productName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, machineStore))
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(ProductLicenseInfo));
                            return (ProductLicenseInfo)serializer.Deserialize(reader);
                        }
                    }
                }
            }

            throw new LicenseNotFoundException("License not found.");
        }

        public UsersPool LoadUsersPoolLicense(string productName)
        {
            UsersPool result = new UsersPool();
            try
            {
                using (var machineStore = IsolatedStorageFile.GetMachineStoreForAssembly())
                {
                    if (machineStore != null)
                    {
                        if (machineStore.FileExists(productName))
                        {
                            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(productName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, machineStore))
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                XmlSerializer serializer = new XmlSerializer(typeof(UsersPool));
                                result = (UsersPool)serializer.Deserialize(reader);
                            }
                        }
                    }
                }
            }
            catch { }

            return result;
        }

        public void SaveLicense(string productName, ProductLicenseInfo licenseInfo)
        {
            lock (_syncRoot)
            {
                if (licenseInfo == null) // Delete the license
                {
                    using (var machineStore = IsolatedStorageFile.GetMachineStoreForAssembly())
                    {
                        if (machineStore != null)
                            machineStore.DeleteFile(productName);
                    }
                    return;
                }

                using (var machineStore = IsolatedStorageFile.GetMachineStoreForAssembly())
                {
                    if (machineStore != null)
                    {
                        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(productName, FileMode.Create, machineStore))
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(ProductLicenseInfo));
                            serializer.Serialize(writer, licenseInfo);
                        }
                    }
                }
            }
        }

        public void SaveUserPoolLicense(string productName, UsersPool usersPool)
        {
            lock (_syncRoot)
            {
                if (usersPool == null) // Delete the users pool
                {
                    using (var machineStore = IsolatedStorageFile.GetMachineStoreForAssembly())
                    {
                        if (machineStore != null)
                            machineStore.DeleteFile(productName);
                    }
                    return;
                }

                using (var machineStore = IsolatedStorageFile.GetMachineStoreForAssembly())
                {
                    if (machineStore != null)
                    {
                        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(productName, FileMode.Create, machineStore))
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(UsersPool));
                            serializer.Serialize(writer, usersPool);
                        }
                    }
                }
            }
        }
    }
}
