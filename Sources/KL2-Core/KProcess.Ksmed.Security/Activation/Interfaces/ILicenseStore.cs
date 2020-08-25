namespace KProcess.Ksmed.Security.Activation
{
    public interface ILicenseStore
    {
        ProductLicenseInfo LoadLicense(string productName);
        void SaveLicense(string productName, ProductLicenseInfo licenseInfo);

        UsersPool LoadUsersPoolLicense(string productName);
        void SaveUserPoolLicense(string productName, UsersPool usersPool);
    }
}
