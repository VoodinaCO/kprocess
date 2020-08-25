using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Security.Activation;

namespace KProcess.Ksmed.Server.Activation.Tests
{
    class UserInformationProviderMock : IUserInformationProvider
    {
        public string Username { get; set; }

        public string Company { get; set; }

        public string Email { get; set; }

        public void SetUserInformation(string username, string company, string email)
        {
            this.Username = username;
            this.Company = company;
            this.Email = email;
        }

        public void Refresh()
        {
        }
    }
}
