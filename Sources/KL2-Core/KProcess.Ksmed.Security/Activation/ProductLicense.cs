using System;
using System.Security.Cryptography;
using System.Text;

namespace KProcess.Ksmed.Security.Activation
{
    #region LicenseStatus
    public enum LicenseStatus
    {
        Licensed,
        TrialVersion,
        Expired,
        MachineHashMismatch,
        NotFound,
        Invalid,
        InternalError
    }
    #endregion

    public class ProductLicense
    {
        private RSACryptoServiceProvider _cryptoService;
        private SHA1CryptoServiceProvider _hashService;
        private IMachineIdentifierProvider _identifierService;
        private IUserInformationProvider _userInfoProvider;

        public ProductLicenseInfo LicenseInfo { get; private set; }

        public short ProductID { get; private set; }
        public short ProductFeatures { get; private set; }
        public short TrialDays { get; private set; }
        public DateTime ProductKeyCreationDate { get; private set; }
        public DateTime ActivationDate { get; private set; }
        public int TrialDaysLeft { get; private set; }

        public string ClientID { get; set; }
        public string UsernameHash { get; set; }
        public string CompanyHash { get; set; }
        public string UserEmailHash { get; set; }

        public LicenseStatus Status { get; private set; }
        public string StatusReason { get; private set; }

        public ProductLicense(LicenseStatus status, string statusReason, short features)
            :this(status, statusReason)
        {
            ProductFeatures = features;
        }

        internal ProductLicense(LicenseStatus status, string statusReason)
        {
            _hashService = new SHA1CryptoServiceProvider();
            Status = status;
            StatusReason = statusReason;
        }

        public ProductLicense(RSACryptoServiceProvider cryptoService, IMachineIdentifierProvider identifierService, ProductLicenseInfo licenseInfo, IUserInformationProvider userInfoProvider)
        {
            _hashService = new SHA1CryptoServiceProvider();
            _cryptoService = cryptoService;
            _identifierService = identifierService;
            _userInfoProvider = userInfoProvider;
            LicenseInfo = licenseInfo;
            ProcessLicense();
        }

        private void ProcessLicense()
        {
            try
            {
                byte[] dataBytes = Convert.FromBase64String(LicenseInfo.ActivationInfo);
                byte[] signBytes = Convert.FromBase64String(LicenseInfo.Signature);

                if (_cryptoService.VerifyData(dataBytes, new SHA1CryptoServiceProvider(), signBytes))
                {
                    int infoLength = 94;

                    // ProductID (2) + ProductFeatures (2) + TrialDays (2) + GeneratedDate (8) + ClientID (12) + UsernameHash (20) + CompanyHash(20) + UserEmailHash(20) + ActivatedDate (8)  = 94

                    byte[] hash = new byte[dataBytes.Length - infoLength];
                    Buffer.BlockCopy(dataBytes, infoLength, hash, 0, hash.Length);

                    if (_identifierService.Match(hash))
                    {
                        ProductID = BitConverter.ToInt16(dataBytes, 0);
                        ProductFeatures = BitConverter.ToInt16(dataBytes, 2);
                        TrialDays = BitConverter.ToInt16(dataBytes, 4);
                        ProductKeyCreationDate = new DateTime(BitConverter.ToInt64(dataBytes, 6));

                        ClientID = Encoding.Unicode.GetString(dataBytes, 14, 12);

                        UsernameHash = Encoding.ASCII.GetString(dataBytes, 26, 20);
                        CompanyHash = Encoding.ASCII.GetString(dataBytes, 46, 20);
                        UserEmailHash = Encoding.ASCII.GetString(dataBytes, 66, 20);

                        ActivationDate = new DateTime(BitConverter.ToInt64(dataBytes, 86));

                        _userInfoProvider.Refresh();
                        if (Hash(_userInfoProvider.Company) == CompanyHash &&
                            Hash(_userInfoProvider.Username) == UsernameHash &&
                            Hash(_userInfoProvider.Email) == UserEmailHash)
                        {
                            if (TrialDays == 0)
                            {
                                Status = LicenseStatus.Licensed;
                                StatusReason = string.Empty;
                                TrialDaysLeft = int.MaxValue;
                            }
                            else
                            {
                                // Modif tekigo : évite en partie les changements de date
                                if (DateTimeOffset.Now.Date < ActivationDate.Date)
                                {
                                    Status = LicenseStatus.Invalid;
                                    StatusReason = "Failed.";
                                }

                                TrialDaysLeft = (TrialDays - (DateTimeOffset.Now.Date - ActivationDate.Date).Days);
                                if (TrialDaysLeft > 0)
                                {
                                    Status = LicenseStatus.TrialVersion;
                                    StatusReason = string.Format("{0} days left.", TrialDaysLeft);
                                }
                                else
                                {
                                    Status = LicenseStatus.Expired;
                                    StatusReason = string.Format("License expired {0} days ago.", -TrialDaysLeft);
                                }
                            }
                        }
                        else
                        {
                            Status = LicenseStatus.Invalid;
                            StatusReason = string.Format(@"User information invalid.
Company: Value: '{0}', Value Hash: {1}, LicenseHash: {2}
Username: Value: '{3}', Value Hash: {4}, LicenseHash: {5}
Email: Value: '{6}', Value Hash: {7}, LicenseHash: {8}",
                                _userInfoProvider.Company, Hash(_userInfoProvider.Company), CompanyHash,
                                _userInfoProvider.Username, Hash(_userInfoProvider.Username), UsernameHash,
                                _userInfoProvider.Email, Hash(_userInfoProvider.Email), UserEmailHash
                                );
                        }
                    }
                    else
                    {
                        Status = LicenseStatus.MachineHashMismatch;
                        StatusReason = "Machine and product info hash mismatch.";
                    }
                }
                else
                {
                    Status = LicenseStatus.Invalid;
                    StatusReason = "Failed verifying signature.";
                }
            }
            catch (Exception ex)
            {
                Status = LicenseStatus.Invalid;
                StatusReason = ex.Message;
            }
        }

        private string Hash(string info)
        {
            return Encoding.ASCII.GetString(_hashService.ComputeHash(Encoding.ASCII.GetBytes(info)));
        }
    }
}
