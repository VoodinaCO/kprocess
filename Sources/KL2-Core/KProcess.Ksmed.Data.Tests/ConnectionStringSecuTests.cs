using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace KProcess.Ksmed.Data.Tests
{
    [TestClass()]
    public class ConnectionStringSecuTests
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        private const string _passphrase = "2_4*Pu*v\"Oz1DUYUGYEXEJ9`";
        private const string _iv = "087DEE58BC80406A";

        /// <summary>
        /// Test qui vérifie si le mot de passe crypté dans le fichier de config permet de se connecter à la bdd.
        /// </summary>
        [TestMethod()]
        [DeploymentItem("KProcess.Ksmed.Data.dll")]
        public void ConnectionStringsSecurityTest()
        {
            var connection = ConnectionStringsSecurity.GetConnectionString(KsmedEntities.ConnectionString);

            Assert.IsNotNull(connection);

            connection.Open();

            Assert.AreEqual(System.Data.ConnectionState.Open, connection.State);

            connection.Close();
        }

        /// <summary>
        /// Crée le mot de passe crypté.
        /// </summary>
        [TestMethod()]
        public void CreateTest()
        {
            string pass = "Mot de passe à crypter";

            var data = EncryptTextToMemory(pass, Encoding.Default.GetBytes(_passphrase), Encoding.Default.GetBytes(_iv));

            string output = string.Concat(Array.ConvertAll(data, x => x.ToString("X2")));
            Console.WriteLine(output);

            Assert.IsNotNull(output);
        }

        /// <summary>
        /// Encrypte le texte spécifié.
        /// </summary>
        /// <param name="Data">Les données à encrypter.</param>
        /// <param name="Key">La clé.</param>
        /// <param name="IV">L'IV.</param>
        /// <returns>Les données encryptés.</returns>
        private static byte[] EncryptTextToMemory(string Data, byte[] Key, byte[] IV)
        {
            try
            {
                // Create a MemoryStream.
                MemoryStream mStream = new MemoryStream();

                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                CryptoStream cStream = new CryptoStream(mStream,
                    new TripleDESCryptoServiceProvider().CreateEncryptor(Key, IV),
                    CryptoStreamMode.Write);

                // Convert the passed string to a byte array.
                byte[] toEncrypt = new ASCIIEncoding().GetBytes(Data);

                // Write the byte array to the crypto stream and flush it.
                cStream.Write(toEncrypt, 0, toEncrypt.Length);
                cStream.FlushFinalBlock();

                // Get an array of bytes from the 
                // MemoryStream that holds the 
                // encrypted data.
                byte[] ret = mStream.ToArray();

                // Close the streams.
                cStream.Close();
                mStream.Close();

                // Return the encrypted buffer.
                return ret;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }

        }

    }
}
