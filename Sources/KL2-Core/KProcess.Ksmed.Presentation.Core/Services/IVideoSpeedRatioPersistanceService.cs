using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Définit le comportement d'un service permettant de stocker le speed ratio d'une vidéo.
    /// </summary>
    public interface IVideoSpeedRatioPersistanceService : IService
    {

        /// <summary>
        /// Obtient le coefficient vidéo pour le fichier spécifié.
        /// </summary>
        /// <param name="file">Le fichier.</param>
        /// <returns>La valeur</returns>
        double? GetSpeedRatio(Uri file);

        /// <summary>
        /// Persiste la vitesse pour le fichier spécifié.
        /// </summary>
        /// <param name="file">Le fichier.</param>
        /// <param name="ratio">La vitesse.</param>
        void PersistSpeedRatio(Uri file, double ratio);

    }
}
