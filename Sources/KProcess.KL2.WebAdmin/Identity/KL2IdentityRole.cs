using KProcess.Ksmed.Models;
using Microsoft.AspNet.Identity;

namespace KProcess.KL2.WebAdmin.Identity
{
    public class KL2IdentityRole : Role, IRole<string>
    {
        public KL2IdentityRole() : base()
        {

        }

        public KL2IdentityRole(string roleCode)
        {
            RoleCode = roleCode;
        }

        public string Id
        {
            get => RoleCode;
            set => RoleCode = value;
        }

        public string Name
        {
            get => RoleCode;
            set => RoleCode = value;
        }
    }
}