using System;
//using System.ServiceModel;
using System.Security.Cryptography;
using KProcess.Ksmed.Security.Activation.MachineIdentifiers;
using KProcess.Ksmed.Security.Activation.Providers;

namespace KProcess.Ksmed.Security.Activation
{
    public class ProductLicenseManager
    {
        readonly ITraceManager _traceManager;

        RSACryptoServiceProvider _cryptoService;

        private ILicenseStore _licenseStore;
        private IMachineIdentifierProvider _identifierService;
        private IUserInformationProvider _userInfoProvider;

        public static ProductLicenseManager Current { get; private set; }

        public static void Initialize(ITraceManager traceManager, string publicXmlKey)
        {
            Current = new ProductLicenseManager(traceManager, publicXmlKey);
        }

        public ProductLicenseManager(ITraceManager traceManager, string publicXmlKey) :
            this(traceManager, publicXmlKey, new IsolatedStorageStore(), new MachineIdentifierProvider(
                traceManager,
                new MachineMultiFootPrintIdentifierV3(traceManager),
                new MachineMultiFootPrintIdentifierV2(traceManager)),
                new UserInformationProvider(true))
        { }

        public ProductLicenseManager(ITraceManager traceManager, string publicXmlKey, ILicenseStore licenseStore, IMachineIdentifierProvider identifierService, IUserInformationProvider userInfoProvider)
        {
            _traceManager = traceManager;

            _cryptoService = new RSACryptoServiceProvider();
            _cryptoService.FromXmlString(publicXmlKey);

            _licenseStore = licenseStore;
            _identifierService = identifierService;
            _userInfoProvider = userInfoProvider;
        }

        public ProductLicense LoadLicense(string productName)
        {
            try
            {
                ProductLicenseInfo licenseInfo = _licenseStore.LoadLicense(productName);
                ProductLicense productLicense = new ProductLicense(_cryptoService, _identifierService, licenseInfo, _userInfoProvider);
                return productLicense;
            }
            catch (LicenseNotFoundException ex)
            {
                return new ProductLicense(LicenseStatus.NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                return new ProductLicense(LicenseStatus.Invalid, ex.Message);
            }
        }

        public WebProductLicense LoadWebLicense(string productName)
        {
            try
            {
                ProductLicenseInfo licenseInfo = _licenseStore.LoadLicense(productName);
                UsersPool usersPool = _licenseStore.LoadUsersPoolLicense(ActivationConstants.WebUsersPoolName);
                WebProductLicense productLicense = new WebProductLicense(_cryptoService, _identifierService, licenseInfo, _userInfoProvider, usersPool);
                return productLicense;
            }
            catch (LicenseNotFoundException ex)
            {
                return new WebProductLicense(WebLicenseStatus.NotFound, "Common_License_StatusReason_Exception", ex.Message);
            }
            catch (Exception ex)
            {
                return new WebProductLicense(WebLicenseStatus.Invalid, "Common_License_StatusReason_Exception", ex.Message);
            }
        }

        public string GetMachineHash()
        {
            if (_identifierService.MachineHash == null)
                return "N/A";
            else
                return Convert.ToBase64String(_identifierService.MachineHash);
        }

        public ProductLicense ActivateProduct(ProductLicenseInfo licenseInfo)
        {
            try
            {
                if (licenseInfo.Signature != null)
                {
                    ProductLicense productLicense = new ProductLicense(_cryptoService, _identifierService, licenseInfo, _userInfoProvider);
                    return productLicense;
                }
                else
                {
                    return new ProductLicense(LicenseStatus.Invalid, licenseInfo.ActivationInfo);
                }
            }
            catch (Exception ex)
            {
                return new ProductLicense(LicenseStatus.InternalError, ex.Message);
            }
        }

        public WebProductLicense ActivateWebProduct(ProductLicenseInfo licenseInfo)
        {
            try
            {
                if (licenseInfo.Signature != null)
                {
                    UsersPool usersPool = _licenseStore.LoadUsersPoolLicense(ActivationConstants.WebUsersPoolName);
                    WebProductLicense productLicense = new WebProductLicense(_cryptoService, _identifierService, licenseInfo, _userInfoProvider, usersPool);
                    return productLicense;
                }
                else
                {
                    return new WebProductLicense(WebLicenseStatus.Invalid, "Common_License_StatusReason_Exception", licenseInfo.ActivationInfo);
                }
            }
            catch (Exception ex)
            {
                return new WebProductLicense(WebLicenseStatus.InternalError, "Common_License_StatusReason_Exception", ex.Message);
            }
        }

        public void SaveLicense(string productName, ProductLicense license)
        {
            if (String.IsNullOrEmpty(productName))
                throw new ArgumentNullException("ProductName is null or empty.");

            if (license == null)
                throw new ArgumentNullException("ProductLicense is null.");

            if (license.LicenseInfo == null)
                throw new InvalidOperationException("ProductLicense is not valid and can't be saved.");

            _licenseStore.SaveLicense(productName, license.LicenseInfo);
        }

        public void SaveWebLicense(string productName, WebProductLicense license)
        {
            if (string.IsNullOrEmpty(productName))
                throw new ArgumentNullException("ProductName is null or empty.");

            if (license !=null && license.LicenseInfo == null)
                throw new InvalidOperationException("ProductLicense is not valid and can't be saved.");

            if (license == null)
                _licenseStore.SaveUserPoolLicense(ActivationConstants.WebUsersPoolName, null);
            else if (license.UsersPool != null)
                _licenseStore.SaveUserPoolLicense(ActivationConstants.WebUsersPoolName, license.UsersPool);
            _licenseStore.SaveLicense(productName, license?.LicenseInfo);
        }
    }
}
