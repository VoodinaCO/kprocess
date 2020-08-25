using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using KProcess.Ksmed.Presentation.Core.ExcelExport;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Permet de spécifier le format de la colonne lors de l'export.
    /// </summary>
    public class ExportFormatBehavior : Behavior<DataGridColumn>
    {

        /// <summary>
        /// Obtient ou définit le binding permettant d'accéder à la donnée qui sera exportée.
        /// </summary>
        public Binding Binding { get; set; }

        /// <summary>
        /// Obtient ou définit le type de données la cellule.
        /// </summary>
        public CellDataType CellDataType
        {
            get { return (CellDataType)GetValue(CellDataTypeProperty); }
            set { SetValue(CellDataTypeProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="CellDataType"/>.
        /// </summary>
        public static readonly DependencyProperty CellDataTypeProperty =
            DependencyProperty.Register("CellDataType", typeof(CellDataType), typeof(ExportFormatBehavior), new UIPropertyMetadata(CellDataType.String));

        /// <summary>
        /// Obtient ou définit l'entête de la colonne.
        /// </summary>
        /// <remarks>
        /// Si cette propriété n'est pas définie, l'entête du <see cref="DataGridColumn"/> associé sera utilisée.
        /// </remarks>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="Header"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(ExportFormatBehavior), new UIPropertyMetadata(null));

    }
}
