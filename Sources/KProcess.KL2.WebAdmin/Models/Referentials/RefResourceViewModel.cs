using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KProcess.KL2.WebAdmin.Models.Referentials
{
    public class RefResourceViewModel
    {
        public int itemId { get; set; }
        public string ParentId { get; set; }
        [Display(Name = "Libellé")]
        public string Label { get; set; }
        [Display(Name = "Couleur")]
        public string Color { get; set; }
        [Display(Name = "Process")]
        public int? ProcessId { get; set; }
        public string ProcessLabel { get; set; }
        public string ProcessLabelSort { get; set; }
        [Display(Name = "Jugement d'allure")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:#.#}")]
        public double PaceRating { get; set; }
        [Display(Name = "Adresse du fichier attaché")]
        public string Uri => TusId == null && Hash == null ? null : $"{System.Web.Configuration.WebConfigurationManager.AppSettings["FileServerUri"]}/GetFile/{Hash}{Extension}";
        public string FileType { get; set; }
        public string Description { get; set; }
        [Display(Name = "Type par défaut")]
        public string ActionTypeCode { get; set; }
        public string ActionTypeLabel { get; set; }
        [Display(Name = "Valorisation")]
        public string ActionValueCode { get; set; }
        public string ActionValueLabel { get; set; }
        public string Hash { get; set; }
        public string TusId { get; set; }
        public string Extension { get; set; }
        public double? CloudFileSize { get; set; }
        public bool IsProcessLink { get; set; }


        public ProcessReferentialIdentifier RefIdentifier { get; set; }
        public int intRefIdentifier { get; set; }
        public List<ProcedureViewModel> ProcedureViewModels { get; set; }

        //public bool HasChild { get; set; }
        //public bool IsExpanded { get; set; }

    }
}