using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Définit le comportement d'un service capable de sauvegarder et de charger le Layout des grilles.
    /// </summary>
    public interface ILayoutPersistanceService : IPresentationService
    {

        /// <summary>
        /// Tente de récupérer le layout sauvegardé.
        /// </summary>
        /// <param name="gridName">Le nom de la grille.</param>
        /// <param name="columns">Les colonnes de la grille.</param>
        /// <param name="rows">Les lignes de la grilles.</param>
        /// <returns><c>true</c> si la récupération a réussi.</returns>
        bool GridTryRetrieve(string gridName, out GridLength[] columns, out GridLength[] rows);

        /// <summary>
        /// Persiste le layout pour la grille spécifiée
        /// </summary>
        /// <param name="gridName">Le nom de la grille.</param>
        /// <param name="columns">Les colonnes de la grille.</param>
        /// <param name="rows">Les lignes de la grilles.</param>
        void GridPersist(string gridName, IEnumerable<GridLength> columns, IEnumerable<GridLength> rows);

        /// <summary>
        /// Tente de récupérer le layout sauvegardé pour le DataGrid spécifié.
        /// </summary>
        /// <returns>
        ///   Le layout, ou <c>null</c> s'il n'est pas défini.
        /// </returns>
        IDictionary<GanttGridView, DataGridLayout> DataGridTryRetrieve();

        /// <summary>
        /// Persiste le layout pour le datagrid spécifiée
        /// </summary>
        /// <param name="layout">Le layout.</param>
        void DataGridPersist(IDictionary<GanttGridView, DataGridLayout> layout);
    }


    /// <summary>
    /// Représente le layout d'un DataGrid.
    /// </summary>
    public class DataGridLayout
    {
        /// <summary>
        /// Obtient ou définit la visibilité des colonnes.
        /// </summary>
        public IDictionary<string, bool> ColumnsVisibilities { get; set; }

        /// <summary>
        /// Obtient ou définit l'ordre des colonnes.
        /// </summary>
        public string[] ColumnsOrder { get; set; }
    }

}
