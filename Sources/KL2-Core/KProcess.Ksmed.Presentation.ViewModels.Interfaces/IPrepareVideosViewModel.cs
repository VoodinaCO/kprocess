using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{
    /// <summary>
    /// Définit le comportement du modèle de vue de l'écran de gestion des vidéos d'un projet.
    /// </summary>
    [InheritedExportAsPerCall]
    public interface IPrepareVideosViewModel : IViewModel, IFrameContentViewModel
    {

        /// <summary>
        /// Obtient la commande permettant d'ajouter une vidéo provenant d'une autre projet.
        /// </summary>
        //ICommand AddOtherProjectVideoCommand { get; }

        /// <summary>
        /// Obtient la commande permettant de sélectionner la vidéo.
        /// </summary>
        ICommand BrowseCommand { get; }

        /// <summary>
        /// Obtient une valeur indiquant si la sélection peut être changée.
        /// </summary>
        bool CanChange { get; }

        /// <summary>
        /// Obtient ou définit la vidéo courante.
        /// </summary>
        Video CurrentVideo { get; set; }

        /// <summary>
        /// Obtient une valeur indiquant si les chemins des vidéos sont en lecture seule.
        /// </summary>
        bool IsVideosPathReadOnly { get; }

        /// <summary>
        /// Obtient les vidéos présentes dans d'autres projets.
        /// </summary>
        //Video[] OtherProjectsVideos { get; }

        /// <summary>
        /// Obtient ou définit les resources disponibles pour la sélection de la ressource par défaut.
        /// </summary>
        BulkObservableCollection<Resource> Resources { get; }

        /// <summary>
        /// Obtient les types de ressources.
        /// </summary>
        ResourceType[] ResourceTypes { get; }

        /// <summary>
        /// Obtient les vidéos.
        /// </summary>
        BindingList<Video> Videos { get; }

    }
}
