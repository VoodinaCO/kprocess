using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using DlhSoft.Windows.Controls;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{
    /// <summary>
    /// Définit le comportement du modèle de vue de l'écran d'acquisition du scénario principal.
    /// </summary>
    [InheritedExportAsPerCall]
    public interface IAcquireViewModel : IGanttGridViewModel<DataTreeGridItem, ActionGridItem, ReferentialGridItem>
    {
        /// <summary>
        /// Obtient les libellés d'actions déjà utilisés.
        /// </summary>
        BulkObservableCollection<string> ActionsLabels { get; }

        /// <summary>
        /// Obtient la commande permettant d'ajouter une action en tant qu'enfant de l'élément sélectionné.
        /// </summary>
        ICommand AddAsChildCommand { get; }

        /// <summary>
        /// Obtient la commande permettant d'ajouter un nouveau référentiel.
        /// </summary>
        ICommand AddReferentialCommand { get; }

        /// <summary>
        /// Désectionne tous les items de la grille courante
        /// </summary>
        ICommand UnselectItemCommand { get; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les markers doivent être liés lors de leurs déplacements.
        /// </summary>
        bool AreMarkersLinked { get; set; }

        /// <summary>
        /// Obtient une valeur indiquant si les timings (début/durée/fin) sont en lecture seule.
        /// </summary>
        bool AreTimingsReadOnly { get; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les timings (début/durée/fin) sont affichés.
        /// </summary>
        bool AreTimingsVisible { get; }

        /// <summary>
        /// Obtient une valeur indiquant la saisie est à la volée.
        /// </summary>
        bool AutoPause { get; set; }

        /// <summary>
        /// Obtient une valeur indiquant si la vidéo d'une action peut être changée à tout moment.
        /// </summary>
        bool CanChangeActionVideo { get; }

        /// <summary>
        /// Obtient les marqueurs à afficher.
        /// </summary>
        ActionGridItem[] CurrentMarkers { get; }

        /// <summary>
        /// Obtient ou définit la vidéo courante.
        /// </summary>
        Video CurrentVideo { get; set; }

        /// <summary>
        /// Obtient la position dans la vidéo courante.
        /// </summary>
        long CurrentVideoPosition { get; set; }

        /// <summary>
        /// Obtient les équipements.
        /// </summary>
        BulkObservableCollection<Equipment> Equipments { get; }

        /// <summary>
        /// Obtient la commande permettant de grouper les éléments.
        /// </summary>
        ICommand GroupCommand { get; }

        /// <summary>
        /// Obtient une valeur indiquant si la saisie est manuelle (sans vidéo associée).
        /// </summary>
        bool IsManualInput { get; }

        /// <summary>
        /// Détermine si le mode lier/délier les marquers est disponible
        /// </summary>
        bool IsMarkersLinkedModeEnabled { get; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la vue associée est chargée.
        /// </summary>
        bool IsViewLoaded { get; set; }

        /// <summary>
        /// Obtient les opérateurs.
        /// </summary>
        BulkObservableCollection<Operator> Operators { get; }

        /// <summary>
        /// Obtient ou définit le marqueur sélectionné.
        /// </summary>
        ActionGridItem SelectedMarker { get; set; }

        /// <summary>
        /// Obtient la commande permettant de dégrouper un groupe.
        /// </summary>
        ICommand UngroupCommand { get; }

        /// <summary>
        /// Obtient la commande permettant de délier l'action courante de la vidéo.
        /// </summary>
        ICommand UnlinkVideoCommand { get; }

        /// <summary>
        /// Obtient les vidéos.
        /// </summary>
        Video[] Videos { get; }


    }
}
