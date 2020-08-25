using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Business.Dtos
{
    /// <summary>
    /// Contient des informations sur les utilisateurs ayant fait des modifications.
    /// </summary>
    public class ModificationsUsers
    {

        /// <summary>
        /// Obtient ou définit le nom complet de l'utilisateur ayant fait la dernière modification.
        /// </summary>
        public string LastModifiedByFullName { get; set; }

        /// <summary>
        /// Obtient ou définit le nom complet de l'utilisateur ayant créé l'élément.
        /// </summary>
        public string CreatedByFullName { get; set; }

    }
}
