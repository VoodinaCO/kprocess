using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace KProcess.KL2.WebAdmin.Identity
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly int iterationCount;

        public PasswordHasher(int iterationCount = 10000)
        {
            if (iterationCount < 1)
            {   
                throw new ArgumentOutOfRangeException("iterationCount",
                        "Password has iteration count cannot be less than 1");
            }
            this.iterationCount = iterationCount;
        }

        public string HashPassword(string password)
        {
            var hash = new System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(
                Encoding.Default.GetBytes(password));
            return Convert.ToBase64String(hash);
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            var hashProvidedPassword = new System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(
               Encoding.Default.GetBytes(providedPassword));

            var hashProvidedPasswordBase64 = Convert.ToBase64String(hashProvidedPassword);

            if (hashProvidedPasswordBase64 == hashedPassword)
            {
                return PasswordVerificationResult.Success;
            }
            return PasswordVerificationResult.Failed;
        }
    }
}