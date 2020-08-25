using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Contient des constantes
    /// </summary>
    public static class PresentationConstants
    {

        /// <summary>
        /// Contient l'adresse de contact
        /// </summary>
        public const string ContactEmail = "KL2Suite@k-process.com";

        /// <summary>
        /// L'adresse du site web
        /// </summary>
        public const string Website = "http://www.k-process.com/";

        /// <summary>
        /// Obtient le dossier où se trouve l'assembly Core. 
        /// </summary>
        public static readonly string AssemblyDirectory =
           System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(PresentationConstants)).Location);

        /// <summary>
        /// Obtient le numéro IDDN.
        /// </summary>
        public static string IDDN = "IDDN.FR.001.140014.002.S.C.2012.000.20700";

    }
}
