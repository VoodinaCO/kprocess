using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace KProcess.Ksmed.Security.Activation
{
    #region LicenseStatus

    public enum WebLicenseStatus
    {
        Licensed,
        TrialVersion,
        Expired,
        MachineHashMismatch,
        NotFound,
        Invalid,
        InternalError,
        OverageOfUsers
    }

    #endregion

    public class WebProductLicense
    {
        RSACryptoServiceProvider _cryptoService;
        SHA1CryptoServiceProvider _hashService;
        IMachineIdentifierProvider _identifierService;
        IUserInformationProvider _userInfoProvider;

        [JsonProperty]
        public ProductLicenseInfo LicenseInfo { get; private set; }

        [JsonProperty]
        public short ProductID { get; private set; }
        [JsonProperty]
        public short TrialDays { get; private set; }
        [JsonProperty]
        public short NumberOfUsers { get; private set; }
        [JsonProperty]
        public DateTime ProductKeyCreationDate { get; private set; }
        [JsonProperty]
        public DateTime ActivationDate { get; private set; }
        [JsonProperty]
        public int TrialDaysLeft { get; private set; }

        [JsonProperty]
        public string UsernameHash { get; set; }
        [JsonProperty]
        public string CompanyHash { get; set; }
        [JsonProperty]
        public string UserEmailHash { get; set; }

        [JsonProperty]
        public UsersPool UsersPool { get; private set; }

        [JsonProperty]
        public WebLicenseStatus Status { get; private set; }
        [JsonProperty]
        public string StatusReason { get; private set; }
        [JsonProperty]
        public object[] StatusReasonParams { get; private set; }

        public WebProductLicense()
        {
            _hashService = new SHA1CryptoServiceProvider();
        }

        public WebProductLicense(WebLicenseStatus status, string statusReason, params string[] statusReasonParams)
        {
            _hashService = new SHA1CryptoServiceProvider();
            Status = status;
            StatusReason = statusReason;
            StatusReasonParams = statusReasonParams;
        }

        public WebProductLicense(RSACryptoServiceProvider cryptoService, IMachineIdentifierProvider identifierService, ProductLicenseInfo licenseInfo, IUserInformationProvider userInfoProvider, UsersPool usersPool)
        {
            _hashService = new SHA1CryptoServiceProvider();
            _cryptoService = cryptoService;
            _identifierService = identifierService;
            _userInfoProvider = userInfoProvider;
            LicenseInfo = licenseInfo;
            UsersPool = usersPool;
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
                    int infoLength = 82;

                    // ProductID (2) + TrialDays (2) + NumberOfUsers (2) + GeneratedDate (8) + UsernameHash (20) + CompanyHash(20) + UserEmailHash(20) + ActivatedDate (8)  = 82

                    byte[] hash = new byte[dataBytes.Length - infoLength];
                    Buffer.BlockCopy(dataBytes, infoLength, hash, 0, hash.Length);

                    if (_identifierService.Match(hash))
                    {
                        ProductID = BitConverter.ToInt16(dataBytes, 0);
                        TrialDays = BitConverter.ToInt16(dataBytes, 2);
                        NumberOfUsers = BitConverter.ToInt16(dataBytes, 4);
                        ProductKeyCreationDate = new DateTime(BitConverter.ToInt64(dataBytes, 6));

                        UsernameHash = Encoding.ASCII.GetString(dataBytes, 14, 20);
                        CompanyHash = Encoding.ASCII.GetString(dataBytes, 34, 20);
                        UserEmailHash = Encoding.ASCII.GetString(dataBytes, 54, 20);

                        ActivationDate = new DateTime(BitConverter.ToInt64(dataBytes, 74));

                        _userInfoProvider.Refresh();
                        if (Hash(_userInfoProvider.Company) == CompanyHash &&
                            Hash(_userInfoProvider.Username) == UsernameHash &&
                            Hash(_userInfoProvider.Email) == UserEmailHash)
                        {
                            if (TrialDays == 0)
                            {
                                Status = WebLicenseStatus.Licensed;
                                StatusReason = "Common_License_StatusReason_Licensed";
                                TrialDaysLeft = int.MaxValue;
                            }
                            else
                            {
                                if (DateTimeOffset.Now.Date < ActivationDate.Date)
                                {
                                    Status = WebLicenseStatus.Invalid;
                                    StatusReason = "Common_License_StatusReason_DateChangeDetected";
                                }

                                TrialDaysLeft = (TrialDays - (DateTimeOffset.Now.Date - ActivationDate.Date).Days);
                                if (TrialDaysLeft > 0)
                                {
                                    Status = WebLicenseStatus.TrialVersion;
                                    StatusReason = "Common_License_StatusReason_Trial";
                                    StatusReasonParams = new object[] { TrialDaysLeft };
                                }
                                else
                                {
                                    Status = WebLicenseStatus.Expired;
                                    StatusReason = "Common_License_StatusReason_Expired";
                                    StatusReasonParams = new object[] { -TrialDaysLeft };
                                }

                                if (UsersPool.Count > NumberOfUsers)
                                {
                                    Status = WebLicenseStatus.OverageOfUsers;
                                    StatusReason = "Common_License_StatusReason_OverageOfUsers";
                                    StatusReasonParams = new object[] { UsersPool.Count, NumberOfUsers };
                                }
                            }
                        }
                        else
                        {
                            Status = WebLicenseStatus.Invalid;
                            StatusReason = "Common_License_StatusReason_InvalidUserInformation";
                            StatusReasonParams = new object[] {
                                _userInfoProvider.Company,
                                _userInfoProvider.Username,
                                _userInfoProvider.Email
                            };
                        }
                    }
                    else
                    {
                        Status = WebLicenseStatus.MachineHashMismatch;
                        StatusReason = "Common_License_StatusReason_MachineHashMismatch";
                    }
                }
                else
                {
                    Status = WebLicenseStatus.Invalid;
                    StatusReason = "Common_License_StatusReason_SignatureFailed";
                }
            }
            catch (Exception ex)
            {
                Status = WebLicenseStatus.Invalid;
                StatusReason = "Common_License_StatusReason_Exception";
                StatusReasonParams = new object[] { ex.Message };
            }
        }

        private string Hash(string info)
        {
            return Encoding.ASCII.GetString(_hashService.ComputeHash(Encoding.ASCII.GetBytes(info)));
        }
    }
}
