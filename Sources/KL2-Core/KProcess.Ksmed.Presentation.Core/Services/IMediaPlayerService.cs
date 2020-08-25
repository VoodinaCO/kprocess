using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows.Controls;
using KProcess.Presentation.Windows;
using System.IO;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Définit le comportement d'un service permettant de récupérer des informations sur le lecteur vidéo courant.
    /// </summary>
    public interface IMediaPlayerService : IPresentationService
    {
        /// <summary>
        /// Obtient le format de la vidéo en cours de lecture.
        /// </summary>
        /// <returns>Le format ou null s'il est inaccessible ou si aucune vidéo n'est chargée.</returns>
        MediaInfo GetFormat();

        /// <summary>
        /// Obtient la durée de la vidéo en cours de lecture.
        /// </summary>
        /// <returns>la durée ou null si aucune vidéo n'est chargée.</returns>
        TimeSpan? GetDuration();

        /// <summary>
        /// Obtient une capture de la vidéo en cours sans annotations
        /// </summary>
        /// <param name="stream"></param>
        void GetThumbnail(Stream stream);

        /// <summary>
        /// Obtient une capture de la vidéo en cours avec des annotations
        /// </summary>
        /// <param name="stream"></param>
        void GetThumbnailWithAnnotations(Stream stream);

        /// <summary>
        /// Remet à zéro les annotations
        /// </summary>
        void ResetAnnotations();

        /// <summary>
        /// Obtient l'état du lecteur
        /// </summary>
        /// <returns></returns>
        bool IsPlaying();
    }
}
