using KProcess.Ksmed.Security.Activation;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace KProcess.Ksmed.Server.Activation
{
    public class ProductKeyPublisher
    {
        RSACryptoServiceProvider _cryptoService;

        #region Constructor

        public ProductKeyPublisher(string privateXmlKey)
        {
            _cryptoService = new RSACryptoServiceProvider();
            _cryptoService.FromXmlString(privateXmlKey);
            if (_cryptoService.PublicOnly)
            {
                throw new CryptographicException("A private key must be provided.");
            }
        }
        public ProductKeyPublisher(RSACryptoServiceProvider cryptoService)
        {
            _cryptoService = cryptoService;
        }

        #endregion

        #region GenerateProductKey

        public string GenerateProductKey(short productID, short productFeatures, short trialDays, string clientID, string username, string company, string userEmail)
        {
            Assertion.NotNull(clientID, "clientID");
            Assertion.NotNull(username, "username");
            Assertion.NotNull(company, "company");
            Assertion.NotNull(userEmail, "userEmail");

            if (clientID.Length > 6)
                throw new ArgumentException("Le clientID ne doit pas excéder 5 caractères.");

            try
            {
                var shaHasher = new SHA1CryptoServiceProvider();

                byte[] proid = BitConverter.GetBytes(productID);
                byte[] pinfo = BitConverter.GetBytes(productFeatures);
                byte[] xinfo = BitConverter.GetBytes(trialDays);
                byte[] ticks = BitConverter.GetBytes(DateTime.Now.Ticks);
                byte[] clientIDBytes = new byte[12]; Encoding.Unicode.GetBytes(clientID).CopyTo(clientIDBytes, 0); // Taille : 12
                byte[] usernameBytes = shaHasher.ComputeHash(Encoding.ASCII.GetBytes(username)); // taille : 20
                byte[] companyBytes = shaHasher.ComputeHash(Encoding.ASCII.GetBytes(company)); // taille : 20
                byte[] userEmailBytes = shaHasher.ComputeHash(Encoding.ASCII.GetBytes(userEmail)); // taille : 20

                System.Diagnostics.Debug.Assert(clientIDBytes.Length == 12);
                System.Diagnostics.Debug.Assert(usernameBytes.Length == 20);
                System.Diagnostics.Debug.Assert(companyBytes.Length == 20);
                System.Diagnostics.Debug.Assert(userEmailBytes.Length == 20);

                byte[] hiddenData;
                using (MemoryStream memStream = new MemoryStream())
                {
                    memStream.Write(proid, 0, 2);
                    memStream.Write(pinfo, 0, 2);
                    memStream.Write(xinfo, 0, 2);
                    memStream.Write(ticks, 0, 8);
                    memStream.Write(clientIDBytes, 0, 12);
                    memStream.Write(usernameBytes, 0, 20);
                    memStream.Write(companyBytes, 0, 20);
                    memStream.Write(userEmailBytes, 0, 20);
                    hiddenData = memStream.ToArray();
                }

                byte[] sign = _cryptoService.SignData(proid, new SHA1CryptoServiceProvider());
                byte[] rkey = new byte[32];
                byte[] rjiv = new byte[16];
                Array.Copy(sign, rkey, 32);
                Array.Copy(sign, 32, rjiv, 0, 16);

                SymmetricAlgorithm algorithm = new RijndaelManaged();
                byte[] hiddenBytes = algorithm.CreateEncryptor(rkey, rjiv).TransformFinalBlock(hiddenData, 0, hiddenData.Length);

                byte[] keyBytes;
                using (MemoryStream stream = new MemoryStream())
                {
                    stream.Write(hiddenBytes, 0, 8);
                    stream.Write(proid, 0, 2);
                    stream.Write(hiddenBytes, 8, hiddenBytes.Length - 8);
                    keyBytes = stream.ToArray();
                }

                string productKey = Convert.ToBase64String(keyBytes);

                return productKey;
            }
            catch (Exception ex)
            {
                return String.Format("Error.{0}", ex.Message);
            }
        }

        public string GenerateWebProductKey(short productID, short trialDays, short numberOfUsers, string username, string company, string userEmail)
        {
            Assertion.NotNull(username, "username");
            Assertion.NotNull(company, "company");
            Assertion.NotNull(userEmail, "userEmail");

            try
            {
                var shaHasher = new SHA1CryptoServiceProvider();

                byte[] proid = BitConverter.GetBytes(productID);
                byte[] pinfo = BitConverter.GetBytes(trialDays);
                byte[] xinfo = BitConverter.GetBytes(numberOfUsers);
                byte[] ticks = BitConverter.GetBytes(DateTime.Now.Ticks);
                byte[] usernameBytes = shaHasher.ComputeHash(Encoding.ASCII.GetBytes(username)); // taille : 20
                byte[] companyBytes = shaHasher.ComputeHash(Encoding.ASCII.GetBytes(company)); // taille : 20
                byte[] userEmailBytes = shaHasher.ComputeHash(Encoding.ASCII.GetBytes(userEmail)); // taille : 20

                System.Diagnostics.Debug.Assert(usernameBytes.Length == 20);
                System.Diagnostics.Debug.Assert(companyBytes.Length == 20);
                System.Diagnostics.Debug.Assert(userEmailBytes.Length == 20);

                byte[] hiddenData;
                using (MemoryStream memStream = new MemoryStream())
                {
                    memStream.Write(proid, 0, 2);
                    memStream.Write(pinfo, 0, 2);
                    memStream.Write(xinfo, 0, 2);
                    memStream.Write(ticks, 0, 8);
                    memStream.Write(usernameBytes, 0, 20);
                    memStream.Write(companyBytes, 0, 20);
                    memStream.Write(userEmailBytes, 0, 20);
                    hiddenData = memStream.ToArray();
                }

                byte[] sign = _cryptoService.SignData(proid, new SHA1CryptoServiceProvider());
                byte[] rkey = new byte[32];
                byte[] rjiv = new byte[16];
                Array.Copy(sign, rkey, 32);
                Array.Copy(sign, 32, rjiv, 0, 16);

                SymmetricAlgorithm algorithm = new RijndaelManaged();
                byte[] hiddenBytes = algorithm.CreateEncryptor(rkey, rjiv).TransformFinalBlock(hiddenData, 0, hiddenData.Length);

                byte[] keyBytes;
                using (MemoryStream stream = new MemoryStream())
                {
                    stream.Write(hiddenBytes, 0, 8);
                    stream.Write(proid, 0, 2);
                    stream.Write(hiddenBytes, 8, hiddenBytes.Length - 8);
                    keyBytes = stream.ToArray();
                }

                string productKey = Convert.ToBase64String(keyBytes);

                return productKey;
            }
            catch (Exception ex)
            {
                return String.Format("Error.{0}", ex.Message);
            }
        }

        #endregion

        #region ValidateProductKey

        public ProductKeyInfo ValidateProductKey(string productKey)
        {
            if (String.IsNullOrEmpty(productKey))
                throw new ArgumentNullException("Product Key is null or empty.");

            byte[] keyBytes;
            try
            {
                keyBytes = Convert.FromBase64String(productKey);
            }
            catch (FormatException e)
            {
                throw new InvalidProductKeyException("Product key is invalid.", e);
            }

            if (keyBytes.Length != 98)
                throw new InvalidProductKeyException("Product key is invalid.");

            byte[] signBytes = new byte[2];
            byte[] hiddenBytes = new byte[96];
            using (MemoryStream stream = new MemoryStream(keyBytes))
            {
                stream.Read(hiddenBytes, 0, 8);
                stream.Read(signBytes, 0, 2);
                stream.Read(hiddenBytes, 8, hiddenBytes.Length - 8);
                keyBytes = stream.ToArray();
            }

            byte[] sign = _cryptoService.SignData(signBytes, new SHA1CryptoServiceProvider());
            byte[] rkey = new byte[32];
            byte[] rjiv = new byte[16];
            Array.Copy(sign, rkey, 32);
            Array.Copy(sign, 32, rjiv, 0, 16);

            SymmetricAlgorithm algorithm = new RijndaelManaged();
            byte[] hiddenData;
            try
            {
                hiddenData = algorithm.CreateDecryptor(rkey, rjiv).TransformFinalBlock(hiddenBytes, 0, hiddenBytes.Length);
            }
            catch (Exception ex)
            {
                throw new InvalidProductKeyException("Product key is invalid.", ex);
            }

            byte[] proid = new byte[2];
            byte[] pinfo = new byte[2];
            byte[] xinfo = new byte[2];
            byte[] ticks = new byte[8];
            byte[] clientIDBytes = new byte[12];
            byte[] usernameBytes = new byte[20];
            byte[] companyBytes = new byte[20];
            byte[] userEmailBytes = new byte[20];

            using (MemoryStream memStream = new MemoryStream(hiddenData))
            {
                memStream.Read(proid, 0, 2);
                memStream.Read(pinfo, 0, 2);
                memStream.Read(xinfo, 0, 2);
                memStream.Read(ticks, 0, 8);
                memStream.Read(clientIDBytes, 0, 12);
                memStream.Read(usernameBytes, 0, 20);
                memStream.Read(companyBytes, 0, 20);
                memStream.Read(userEmailBytes, 0, 20);
            }

            if (signBytes[0] == proid[0] && signBytes[1] == proid[1])
            {
                DateTime generatedDate = new DateTime(BitConverter.ToInt64(ticks, 0));
                if (generatedDate.Year > 2000 && generatedDate.Year < 2100)
                {
                    return new ProductKeyInfo()
                    {
                        ProductID = BitConverter.ToInt16(proid, 0),
                        ProductFeatures = BitConverter.ToInt16(pinfo, 0),
                        TrialDays = BitConverter.ToInt16(xinfo, 0),
                        GeneratedDate = generatedDate,

                        ClientID = Encoding.Unicode.GetString(clientIDBytes),
                        UsernameHash = Encoding.ASCII.GetString(usernameBytes),
                        CompanyHash = Encoding.ASCII.GetString(companyBytes),
                        UserEmailHash = Encoding.ASCII.GetString(userEmailBytes),
                    };
                }
                else
                {
                    throw new InvalidProductKeyException("Product key date is incorrect.");
                }
            }
            else
            {
                throw new InvalidProductKeyException("Product key info is incorrect.");
            }
        }

        public WebProductKeyInfo ValidateWebProductKey(string productKey)
        {
            if (string.IsNullOrEmpty(productKey))
                throw new ArgumentNullException("Product Key is null or empty.");

            byte[] keyBytes;
            try
            {
                keyBytes = Convert.FromBase64String(productKey);
            }
            catch (FormatException e)
            {
                throw new InvalidProductKeyException("Product key is invalid.", e);
            }

            if (keyBytes.Length != 82)
                throw new InvalidProductKeyException("Product key is invalid.");

            byte[] signBytes = new byte[2];
            byte[] hiddenBytes = new byte[80];
            using (MemoryStream stream = new MemoryStream(keyBytes))
            {
                stream.Read(hiddenBytes, 0, 8);
                stream.Read(signBytes, 0, 2);
                stream.Read(hiddenBytes, 8, hiddenBytes.Length - 8);
                keyBytes = stream.ToArray();
            }

            byte[] sign = _cryptoService.SignData(signBytes, new SHA1CryptoServiceProvider());
            byte[] rkey = new byte[32];
            byte[] rjiv = new byte[16];
            Array.Copy(sign, rkey, 32);
            Array.Copy(sign, 32, rjiv, 0, 16);

            SymmetricAlgorithm algorithm = new RijndaelManaged();
            byte[] hiddenData;
            try
            {
                hiddenData = algorithm.CreateDecryptor(rkey, rjiv).TransformFinalBlock(hiddenBytes, 0, hiddenBytes.Length);
            }
            catch (Exception ex)
            {
                throw new InvalidProductKeyException("Product key is invalid.", ex);
            }

            byte[] proid = new byte[2];
            byte[] pinfo = new byte[2];
            byte[] xinfo = new byte[2];
            byte[] ticks = new byte[8];
            byte[] usernameBytes = new byte[20];
            byte[] companyBytes = new byte[20];
            byte[] userEmailBytes = new byte[20];

            using (MemoryStream memStream = new MemoryStream(hiddenData))
            {
                memStream.Read(proid, 0, 2);
                memStream.Read(pinfo, 0, 2);
                memStream.Read(xinfo, 0, 2);
                memStream.Read(ticks, 0, 8);
                memStream.Read(usernameBytes, 0, 20);
                memStream.Read(companyBytes, 0, 20);
                memStream.Read(userEmailBytes, 0, 20);
            }

            if (signBytes[0] == proid[0] && signBytes[1] == proid[1])
            {
                DateTime generatedDate = new DateTime(BitConverter.ToInt64(ticks, 0));
                if (generatedDate.Year > 2000 && generatedDate.Year < 2100)
                {
                    return new WebProductKeyInfo()
                    {
                        ProductID = BitConverter.ToInt16(proid, 0),
                        TrialDays = BitConverter.ToInt16(pinfo, 0),
                        NumberOfUsers = BitConverter.ToInt16(xinfo, 0),
                        GeneratedDate = generatedDate,

                        UsernameHash = Encoding.ASCII.GetString(usernameBytes),
                        CompanyHash = Encoding.ASCII.GetString(companyBytes),
                        UserEmailHash = Encoding.ASCII.GetString(userEmailBytes),
                    };
                }
                else
                {
                    throw new InvalidProductKeyException("Product key date is incorrect.");
                }
            }
            else
            {
                throw new InvalidProductKeyException("Product key info is incorrect.");
            }
        }

        #endregion
    }
}
