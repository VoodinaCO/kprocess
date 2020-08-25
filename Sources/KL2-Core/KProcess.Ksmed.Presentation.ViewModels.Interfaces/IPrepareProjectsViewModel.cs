using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Interfaces;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{

    /// <summary>
    /// Définit le comportement du VM de la gestion de projet.
    /// </summary>
    [InheritedExportAsPerCall]
    public interface IPrepareProjectsViewModel : IFrameContentViewModel
    {
        /// <summary>
        /// Obtient ou définit l''objectif additionel.
        /// </summary>
        string AlternateObjective { get; set; }

        /// <summary>
        /// Obtient ou définit une propriété indiquant si l'utilisateur est authorisé à changer de projet.
        /// </summary>
        bool CanChangeProject { get; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les projets peuvent être importés.
        /// </summary>
        bool CanImportProject { get; }

        /// <summary>
        /// Obtient ou définit le noeud.
        /// </summary>
        INode CurrentNode { get; set; }

        /// <summary>
        /// Obtient ou définit le projet.
        /// </summary>
        Project CurrentProject { get; set; }

        /// <summary>
        /// Obtient la commande permettant d'exporter le projet sélectionné vers excel.
        /// </summary>
        ICommand ExportExcelCommand { get; }

        /// <summary>
        /// Obtient la commande permettant d'exporter un projet.
        /// </summary>
        ICommand ExportProjectCommand { get; }

        /// <summary>
        /// Obtient la commande permettant d'importer un projet.
        /// </summary>
        ICommand ImportProjectCommand { get; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'objectif additionel est coché.
        /// </summary>
        bool IsAlternateObjectiveChecked { get; set; }

        /// <summary>
        /// Obtient une valeur indiquant si le project courant est ouvert.
        /// </summary>
        bool IsCurrentProjectOpened { get; }

        /// <summary>
        /// Obtient une valeur indiquant si l'utilisateur courant est un administrateur.
        /// </summary>
        bool IsCurrentUserAdmin { get; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la licence actuelle est en lecture seule.
        /// </summary>
        bool IsRunningReadOnlyVersion { get; }

        /// <summary>
        /// Obtient ou définit les objectifs disponibles.
        /// </summary>
        Objective[] Objectives { get; set; }

        /// <summary>
        /// Obtient la commande permettant d'ouvrir la liste des projets.
        /// </summary>
        ICommand OpenCommand { get; }

        /// <summary>
        /// Obtient la commande permettant d'ouvrir le projet sélectionné (si celui-ci n'est pas modifié).
        /// </summary>
        ICommand OpenProjectCommand { get; }

        /// <summary>
        /// Obtient ou définit l'arborescence de projets.
        /// </summary>
        BulkObservableCollection<INode> AllProjects { get; set; }

        /// <summary>
        /// Obtient ou définit la visibilité de la liste des projets.
        /// </summary>
        Visibility ProjectsListVisibility { get; set; }

        /// <summary>
        /// Obtient les échelles de temps possibles.
        /// </summary>
        TimeScaleContainer[] TimeScales { get; }

    }

    /// <summary>
    /// Représente un conteneur autour des échelles de temps.
    /// </summary>
    public class TimeScaleContainer
    {
        /// <summary>
        /// Obtient ou définit le libellé.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Obtient ou définit la valeur.
        /// </summary>
        public long Value { get; set; }
    }

}
