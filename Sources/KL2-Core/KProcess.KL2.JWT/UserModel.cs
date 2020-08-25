using System.Collections.Generic;

namespace KProcess.KL2.JWT
{
    public class UserModel
    {
        /// <summary>
        /// User Id.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Firstname.
        /// </summary>
        public string Firstname { get; set; }

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Fullname.
        /// </summary>
        public string FullName => $"{Firstname} {Name}".Trim();

        /// <summary>
        /// User email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// User phone number.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// User default language code.
        /// </summary>
        public string DefaultLanguageCode { get; set; }

        /// <summary>
        /// User current language code.
        /// </summary>
        public string CurrentLanguageCode { get; set; }

        /// <summary>
        /// List of user roles the user belongs to.
        /// </summary>
        public IEnumerable<string> Roles { get; set; }

        /// <summary>
        /// Get or Set if the services have to remember the user.
        /// </summary>
        public bool RememberMe { get; set; }
        public bool IsActive { get; set; }
    }
}
