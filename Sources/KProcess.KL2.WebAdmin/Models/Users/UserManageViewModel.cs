using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Users
{
    public class UserManageViewModel
    {
        public IEnumerable<UserViewModel> UsersViewModel { get; set; }
        public IEnumerable<RoleViewModel> RolesViewModel { get; set; }
        public IEnumerable<LanguageViewModel> LanguagesViewModel { get; set; }
    }
}