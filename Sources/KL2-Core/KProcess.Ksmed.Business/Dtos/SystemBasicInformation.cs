using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Business.Dtos
{
    /// <summary>
    /// Contient des informations basiques sur le système.
    /// </summary>
    public class SystemBasicInformation
    {
        /// <summary>
        /// Obtient ou définit le nom de la machine.
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Obtient ou définit le nom du système d'exploitation.
        /// </summary>
        public string OperatingSystem { get; set; }

        /// <summary>
        /// Obtient ou définit l'architecture du système d'exploitation.
        /// </summary>
        public string OperatingSystemArchitecture { get; set; }

        /// <summary>
        /// Obtient ou définit la version du système d'exploitation.
        /// </summary>
        public string OperatingSystemVersion{ get; set; }

        /// <summary>
        /// Obtient ou définit la langue du système d'exploitation.
        /// </summary>
        public CultureInfo OperatingSystemLanguage { get; set; }

        /// <summary>
        /// Obtient ou définit le fabricant du PC.
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// Obtient ou définit le modèle du PC.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Obtient ou définit les noms des processeurs.
        /// </summary>
        public string[] Processors { get; set; }

        /// <summary>
        /// Obtient ou définit la mémoire vive totale (en octets).
        /// </summary>
        public ulong Memory { get; set; }

        /// <summary>
        /// Obtient ou définit la mémoire vive utilisable par le système d'exploitation (en ko).
        /// </summary>
        public ulong OSVisibleMemory { get; set; }

        /// <summary>
        /// Obtient ou définit les contrôleurs vidéos.
        /// </summary>
        public VideoController[] VideoControllers { get; set; }

    }

    /// <summary>
    /// Représente un contrôleur vidéo.
    /// </summary>
    public class VideoController
    {
        /// <summary>
        /// Obtient ou définit le nom.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Obtient ou définit la résolution.
        /// </summary>
        public string Resolution { get; set; }
    }
}
