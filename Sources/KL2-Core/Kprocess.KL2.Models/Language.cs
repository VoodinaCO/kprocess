//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kprocess.KL2.Models
{
    using System;
    using System.Collections.Generic;
    using TrackableEntities;
    
    public partial class Language : ITrackable, IMergeable
    {
        public Language()
        {
            this.AppResourceValues = new List<AppResourceValue>();
            this.Users = new List<User>();
        }
    
        public string LanguageCode { get; set; }
        public string Label { get; set; }
    
        public ICollection<AppResourceValue> AppResourceValues { get; set; }
        public ICollection<User> Users { get; set; }
    
        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
        public Guid EntityIdentifier { get; set; }
    }
}