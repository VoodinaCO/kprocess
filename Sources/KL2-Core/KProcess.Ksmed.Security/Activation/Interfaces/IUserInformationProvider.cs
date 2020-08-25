using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Security.Activation
{
    public interface IUserInformationProvider
    {
        string Username { get; }
        string Company { get; }
        string Email { get; }

        void Refresh();
        void SetUserInformation(string username, string company, string email);
    }
}
