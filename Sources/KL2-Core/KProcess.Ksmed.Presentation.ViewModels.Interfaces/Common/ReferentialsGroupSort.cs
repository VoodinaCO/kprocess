using KProcess.Globalization;
using KProcess.Ksmed.Models;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace KProcess.Ksmed.Presentation.ViewModels
{

    /// <summary>
    /// Permet de grouper et de trier en fonction du type de référentiel : Standard et par projet.
    /// </summary>
    public class ReferentialsGroupSortDescription : GroupDescription, IComparer
    {
        private string _standard;
        private string _withoutProject;
        private ReferentialsSort _referentialsSort = new ReferentialsSort();

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="AppActionsReferentialsViewModelBase&lt;TItem, TItemStandard, TItemProject&gt;.ReferentialsGroupDescription"/>.
        /// </summary>
        public ReferentialsGroupSortDescription()
        {
            _standard = LocalizationManager.GetString("VM_ReferentialsGroupSortDescription_Standard");
            _withoutProject = LocalizationManager.GetString("VM_ReferentialsGroupSortDescription_New");
        }

        /// <summary>
        /// Retourne le nom du groupe à partir de l'objet.
        /// </summary>
        /// <param name="item">L'objet.</param>
        /// <param name="level">Le niveau du groupe.</param>
        /// <param name="culture">La <see cref="T:System.Globalization.CultureInfo"/> à fournir au convertisseur.</param>
        /// <returns>
        /// Le nom du groupe.
        /// </returns>
        public override object GroupNameFromItem(object item, int level, System.Globalization.CultureInfo culture)
        {
            if ((item as IActionReferentialProcess)?.ProcessId == null)
                return _standard;
            else if ((item as IActionReferentialProcess)?.Process != null)
                return (item as IActionReferentialProcess)?.Process;
            else
                return _withoutProject;
        }

        /// <summary>
        /// Compare deux objets.
        /// </summary>
        /// <param name="x">L'objet 1</param>
        /// <param name="y">L'objet 2</param>
        /// <returns>Le résultat de la comparaison.</returns>
        public int Compare(object x, object y)
        {
            var xStandard = x as IActionReferentialProcess;
            var yStandard = y as IActionReferentialProcess;

            if (xStandard?.ProcessId == null && yStandard?.ProcessId == null)
            {
                if (xStandard == null && yStandard == null)
                    return 0;
                return _referentialsSort.Compare(xStandard, yStandard);
            }
            else if (xStandard?.ProcessId == null && yStandard?.ProcessId != null)
            {
                return -1;
            }
            else if (xStandard?.ProcessId != null && yStandard?.ProcessId == null)
            {
                return 1;
            }

            var xProject = (IActionReferentialProcess)x;
            var yProject = (IActionReferentialProcess)y;

            // Comparer les projets
            if (xProject == null && yProject == null)
                return 0;
            else
            {
                if (xProject.Process == null && yProject.Process == null)
                {
                    // Comparer les libellés
                    return Comparer<string>.Default.Compare(xProject.Label, yProject.Label);
                }
                else if (xProject.Process == null)
                    return -1;
                else if (yProject.Process == null)
                    return 1;
            }

            var projectComp = Comparer<string>.Default.Compare(xProject.Process.Label, yProject.Process.Label);
            if (projectComp != 0)
                return projectComp;


            // Comparer les libellés
            return Comparer<string>.Default.Compare(xProject.Label, yProject.Label);
        }
    }
}
