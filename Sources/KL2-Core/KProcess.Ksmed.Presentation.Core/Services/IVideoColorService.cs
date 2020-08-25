using KProcess.Ksmed.Models;
using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Décrit le comportement d'un service capable d'indiquer la couleur d'une vidéo.
    /// </summary>
    public interface IVideoColorService : IPresentationService
    {

        /// <summary>
        /// Obtient la couleur de la vidéo.
        /// </summary>
        /// <param name="video">La vidéo.</param>
        /// <returns>la couleur.</returns>
        Brush GetColor(Video video);

    }
}
