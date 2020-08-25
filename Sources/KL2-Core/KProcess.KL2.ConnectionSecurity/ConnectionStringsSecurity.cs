using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace KProcess.KL2.ConnectionSecurity
{
    /// <summary>
    /// Gère la sécurité des chaînes de connexion.
    /// </summary>
    public static class ConnectionStringsSecurity
    {
        private const string _passphrase = "2_4*Pu*v\"Oz1DUYUGYEXEJ9`";
        private const string _iv = "087DEE58BC80406A";

        /// <summary>
        /// Décrypte le mot de passe crypté.
        /// </summary>
        /// <param name="encryptedPassword">le mot de passe crypté.</param>
        /// <returns>Le mot de passe décrypté</returns>
        public static string DecryptPassword(string encryptedPassword)
        {
            var Data = ToByteArray(encryptedPassword);

            MemoryStream msDecrypt = new MemoryStream(Data);

            CryptoStream csDecrypt = new CryptoStream(msDecrypt,
                new TripleDESCryptoServiceProvider().CreateDecryptor(Encoding.Default.GetBytes(_passphrase), Encoding.Default.GetBytes(_iv)),
                CryptoStreamMode.Read);

            byte[] fromEncrypt = new byte[Data.Length];

            csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

            fromEncrypt = fromEncrypt.Where(b => b != 0).ToArray();

            return new ASCIIEncoding().GetString(fromEncrypt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string CryptPassword(string password)
        {
            var Data = new ASCIIEncoding().GetBytes(password);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                CryptoStream csEncrypt = new CryptoStream(msEncrypt,
                    new TripleDESCryptoServiceProvider().CreateEncryptor(Encoding.Default.GetBytes(_passphrase), Encoding.Default.GetBytes(_iv)),
                    CryptoStreamMode.Write);
                csEncrypt.Write(Data, 0, Data.Length);
                csEncrypt.FlushFinalBlock();

                byte[] result = msEncrypt.ToArray();
                return ByteArrayToString(result);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string CryptPassword(SecureString securePassword)
        {
            var unsecuredPasswordPtr = IntPtr.Zero;
            try
            {
                unsecuredPasswordPtr = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                string unsecuredPassword = Marshal.PtrToStringUni(unsecuredPasswordPtr);
                return CryptPassword(unsecuredPassword);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unsecuredPasswordPtr);
            }
        }

        /// <summary>
        /// Convertit un packet d'octets représentés en chaîne hexa en un tableau d'octets.
        /// </summary>
        /// <param name="hex">L'héxa.</param>
        /// <returns>Le tableau d'octets</returns>
        public static byte[] ToByteArray(string hex)
        {
            if (hex.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                hex = hex.Substring(2);

            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }
    }
}
