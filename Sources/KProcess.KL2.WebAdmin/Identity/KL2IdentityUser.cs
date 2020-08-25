using KProcess.Ksmed.Models;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KProcess.KL2.WebAdmin.Identity
{
    public class KL2IdentityUser : User, IUser<string>
    {
        public KL2IdentityUser() : base()
        {

        }

        public KL2IdentityUser(User user)
        {
            if (user != null)
            {
                Id = user.UserId.ToString();
                UserName = user.Username;
                Email = user.Email;
                Password = user.Password;
                Firstname = user.Firstname;
                Name = user.Name;
                PhoneNumber = user.PhoneNumber;
                DefaultLanguageCode = user.DefaultLanguageCode;
                CurrentLanguageCode = user.CurrentLanguageCode ?? user.DefaultLanguageCode;
                Roles = user.Roles;
            }
        }

        public KL2IdentityUser(string Username) {
            this.UserName = Username;
        }
        public string Id
        {
            get
            {
                return UserId.ToString();
            }
            set
            {
                UserId = int.Parse(value);
            }
        }
        public string UserName
        {
            get
            {
                return Username;
            }
            set
            {
                Username = value;
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<KL2IdentityUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}