using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Security.Activation;
using System.Security.Cryptography;

namespace KProcess.Ksmed.Server.Activation.Tests
{
    class KeyHelpers
    {
        public static string GetPrivateKey()
        {
            return Helpers.GetResourceString("KProcess.Ksmed.Server.Activation.Tests.PrivateKey.xml");
        }

        public static string GetPublicKey()
        {
            return Helpers.GetResourceString("KProcess.Ksmed.Server.Activation.Tests.PublicKey.xml");
        }

        public static Tuple<string, string> GenerateNewPublicPrivateKeyPair()
        {
            var rsa = new RSACryptoServiceProvider();
            var pu = rsa.ToXmlString(false);
            var pr = rsa.ToXmlString(true);

            return new Tuple<string, string>(pu, pr);
        }
    }
}
