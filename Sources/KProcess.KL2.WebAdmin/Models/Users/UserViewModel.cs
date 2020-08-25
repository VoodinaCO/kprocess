using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace KProcess.KL2.WebAdmin.Models.Users
{
    public class UserViewModel
    {
        [Display(Name = "ID")]
        public int? UserId { get; set; }
        [Required(ErrorMessage = "Veuillez sélectionner un langage")]
        [Display(Name = "Langage")]
        public string DefaultLanguageCode { get; set; }
        
        [Display(Name = "Position")]
        public Nullable<bool> Tenured { get; set; }
        public string TenuredDisplay { get; set; }
        [Required]
        [Display(Name = "Utilisateur")]
        public string Username { get; set; }
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }
        [Display(Name = "Nouveau mot de passe")]
        public string NewPassword { get; set; }
        [Required]
        [Display(Name = "Prénom")]
        public string Firstname { get; set; }
        [Required]
        [Display(Name = "Nom")]
        public string Name { get; set; }
        [Display(Name = "Nom et prénom")]
        public string FullName { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Numéro de téléphone")]
        public string PhoneNumber { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
        [Required(ErrorMessage = "Veuillez sélectionner un role")]
        [Display(Name = "Roles")]
        public List<string> Roles { get; set; }
        public List<int> RolesId { get; set; }
        public List<string> RoleCodes { get; set; }
        public List<string> Teams { get; set; }
        public bool IsActive { get; set; }

        public IEnumerable<LanguageViewModel> LanguagesViewModel { get; set; }

        public int AdministratorCount { get; set; }
    }

    
}