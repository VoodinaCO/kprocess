using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Markup;
using KProcess.Globalization;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core.Behaviors;
using KProcess.Ksmed.Presentation.Core.Converters;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Shell.Views
{
    /// <summary>
    /// Contient des méthodes permattant l'affichage de la synthèse.
    /// </summary>
    static class SummaryBuilder
    {
        /// <summary>
        /// Définit les données dans le dataGrid spécifié afin d'afficher la partie "Scénarios".
        /// </summary>
        /// <param name="summary">Les données de la synthèse.</param>
        /// <param name="dataGrid">Le DataGrid cible.</param>
        public static void BuildScenarios(ScenarioCriticalPath[] summary, DataGrid dataGrid)
        {
            var timeFormatService = IoC.Resolve<IServiceBus>().Get<ITimeTicksFormatService>();

            dataGrid.Columns.Clear();
            var parentLine = new CellContent[summary.Length + 2];
            var islockedLine = new CellContent[summary.Length + 2];
            var durationILine = new CellContent[summary.Length + 2];
            var earningILine = new CellContent[summary.Length + 2];
            var durationIELine = new CellContent[summary.Length + 2];
            var earningIELine = new CellContent[summary.Length + 2];
            var valuesLine = new CellContent[summary.Length + 2];

            int columnIndex = 0;

            durationILine[columnIndex] = new CellContent(LocalizationManager.GetString("View_Shared_Summary_CriticalPath")) { FontWeight = FontWeights.Bold };

            columnIndex++;

            parentLine[columnIndex] = new CellContent(LocalizationManager.GetString("View_Shared_Summary_Original")) { FontWeight = FontWeights.Bold };
            durationILine[columnIndex] = new CellContent(LocalizationManager.GetString("View_Shared_Summary_CriticalPathDurationI")) { FontWeight = FontWeights.Bold };
            earningILine[columnIndex] = new CellContent(LocalizationManager.GetString("View_Shared_Summary_CriticalPathEarningI")) { FontWeight = FontWeights.Bold };
            durationIELine[columnIndex] = new CellContent(LocalizationManager.GetString("View_Shared_Summary_CriticalPathDurationIE")) { FontWeight = FontWeights.Bold };
            earningIELine[columnIndex] = new CellContent(LocalizationManager.GetString("View_Shared_Summary_CriticalPathEarningIE")) { FontWeight = FontWeights.Bold };
            valuesLine[columnIndex] = new CellContent(LocalizationManager.GetString("View_Shared_Summary_Values")) { FontWeight = FontWeights.Bold };

            AddColumn(dataGrid.Columns, columnIndex);

            foreach (var scenario in summary.OrderBy(s => s.Id).ToArray())
            {
                columnIndex++;

                AddColumn(dataGrid.Columns, columnIndex, scenario.Label, true);

                if (scenario.OriginalLabel != null)
                    parentLine[columnIndex] = scenario.OriginalLabel;

                if (scenario.IsLocked)
                    islockedLine[columnIndex] =
                        new CellContent(new ContentControl
                        {
                            ContentTemplate = dataGrid.FindResource("SummaryLockedCellTemplate") as DataTemplate,
                        })
                        {
                            TextContent = scenario.IsLocked ? "X" : null
                        };

                durationILine[columnIndex] = new CellContent(timeFormatService.TicksToString(scenario.CriticalPathDuration)) { HorizontalAlignment = HorizontalAlignment.Center, FontWeight = FontWeights.Bold };

                if (scenario.EarningPercent.HasValue)
                    earningILine[columnIndex] = new CellContent(string.Format("{0:0.0}%", scenario.EarningPercent)) { HorizontalAlignment = HorizontalAlignment.Center, FontWeight = FontWeights.Bold };

                if (scenario.CriticalPathDurationIE.HasValue)
                    durationIELine[columnIndex] = new CellContent(timeFormatService.TicksToString(scenario.CriticalPathDurationIE.Value)) { HorizontalAlignment = HorizontalAlignment.Center, FontWeight = FontWeights.Bold };

                if (scenario.EarningPercentIE.HasValue)
                    earningIELine[columnIndex] = new CellContent(string.Format("{0:0.0}%", scenario.EarningPercentIE)) { HorizontalAlignment = HorizontalAlignment.Center, FontWeight = FontWeights.Bold };

                valuesLine[columnIndex] = new CellContent(string.Format(LocalizationManager.GetString("View_Shared_Summary_Values_StringFormat"),
                    scenario.Values[KnownActionCategoryValues.VA],
                    scenario.Values[KnownActionCategoryValues.BNVA],
                    scenario.Values[KnownActionCategoryValues.NVA],
                    scenario.Values[ScenarioCriticalPath.ValueNoneKey]
                    )) { HorizontalAlignment = HorizontalAlignment.Center, FontWeight = FontWeights.Bold };


            }

            dataGrid.ItemsSource = new object[][]
            {
                parentLine,
                islockedLine,
                durationILine,
                earningILine,
                durationIELine,
                earningIELine,
                valuesLine,
            };
        }

        /// <summary>
        /// Définit les données dans le dataGrid spécifié afin d'afficher la partie "Equipements" ou "Opérateurs".
        /// </summary>
        /// <param name="summary">Les données de la synthèse.</param>
        /// <param name="dataGrid">Le DataGrid cible.</param>
        /// <param name="equipments"><c>true</c> pour les équipements, sinon <c>false</c>.</param>
        public static void BuildResources(ScenarioCriticalPath[] summary, DataGrid dataGrid, bool equipments)
        {
            var timeFormatService = IoC.Resolve<IServiceBus>().Get<ITimeTicksFormatService>();

            dataGrid.Columns.Clear();
            string[] distinctResourcesLabels;
            if (equipments)
            {
                distinctResourcesLabels = summary
                    .SelectMany(s => s.Equipments)
                    .Select(cp => cp.Label)
                    .Distinct()
                    .ToArray();
            }
            else
            {
                distinctResourcesLabels = summary
                    .SelectMany(s => s.Operators)
                    .Select(cp => cp.Label)
                    .Distinct()
                    .ToArray();
            }

            int rowIndex = 0;
            var rows = new CellContent[4 + 5 * distinctResourcesLabels.Length][]; // 4 = parent + islocked + totalduration + totalearning

            var parentLine = new CellContent[summary.Length + 2];
            var islockedLine = new CellContent[summary.Length + 2];

            int columnIndex = 0;

            AddColumn(dataGrid.Columns, columnIndex);

            columnIndex++;
            parentLine[columnIndex] = LocalizationManager.GetString("View_Shared_Summary_Original");

            AddColumn(dataGrid.Columns, columnIndex);

            // Renseigner tout d'abord les trois premieres lignes :
            // Label
            // OriginalLabel
            // IsLocked
            foreach (var scenario in summary)
            {
                columnIndex++;

                AddColumn(dataGrid.Columns, columnIndex, scenario.Label, true);

                if (scenario.OriginalLabel != null)
                    parentLine[columnIndex] = scenario.OriginalLabel;

                if (scenario.IsLocked)
                    islockedLine[columnIndex] =
                        new CellContent(new ContentControl
                        {
                            ContentTemplate = dataGrid.FindResource("SummaryLockedCellTemplate") as DataTemplate,
                        })
                        {
                            TextContent = scenario.IsLocked ? "X" : null
                        };
            }

            rows[rowIndex] = parentLine;
            rowIndex++;
            rows[rowIndex] = islockedLine;
            rowIndex++;

            // Afficher la ligne de total
            var totalDurationLine = new CellContent[summary.Length + 2];
            var totalEarningLine = new CellContent[summary.Length + 2];

            columnIndex = 0;

            totalDurationLine[columnIndex] = new CellContent(LocalizationManager.GetString("View_PrepareScenarios_Summary_Total")) { FontWeight = FontWeights.Bold };
            columnIndex++;

            totalDurationLine[columnIndex] = new CellContent(LocalizationManager.GetString("View_Shared_Summary_CriticalPathDuration")) { FontWeight = FontWeights.Bold };
            totalEarningLine[columnIndex] = new CellContent(LocalizationManager.GetString("View_Shared_Summary_CriticalPathEarning")) { FontWeight = FontWeights.Bold };
            columnIndex++;

            foreach (var scenario in summary)
            {
                ResourceCriticalPath total;
                if (equipments)
                    total = scenario.EquipmentsTotal;
                else
                    total = scenario.OperatorsTotal;

                totalDurationLine[columnIndex] = new CellContent(timeFormatService.TicksToString(total.Duration)) { HorizontalAlignment = HorizontalAlignment.Center, FontWeight = FontWeights.Bold };
                totalEarningLine[columnIndex] = new CellContent(string.Format("{0:F}%", total.EarningPercent)) { HorizontalAlignment = HorizontalAlignment.Center, FontWeight = FontWeights.Bold };

                columnIndex++;
            }

            rows[rowIndex] = totalDurationLine;
            rowIndex++;
            rows[rowIndex] = totalEarningLine;
            rowIndex++;

            // Afficher maintenant toutes les ressources

            foreach (var resourceLabel in distinctResourcesLabels)
            {
                columnIndex = 0;

                var durationLine = new CellContent[summary.Length + 2];
                var earningLine = new CellContent[summary.Length + 2];
                var loadLine = new CellContent[summary.Length + 2];
                var overloadLine = new CellContent[summary.Length + 2];
                var valuesLine = new CellContent[summary.Length + 2];

                durationLine[columnIndex] = resourceLabel;
                columnIndex++;

                durationLine[columnIndex] = LocalizationManager.GetString("View_Shared_Summary_CriticalPathDuration");
                earningLine[columnIndex] = LocalizationManager.GetString("View_Shared_Summary_CriticalPathEarning");
                loadLine[columnIndex] = LocalizationManager.GetString("View_PrepareScenarios_Summary_TxOccCC");
                overloadLine[columnIndex] = LocalizationManager.GetString("View_PrepareScenarios_Summary_TxSup");
                valuesLine[columnIndex] = LocalizationManager.GetString("View_Shared_Summary_Values");

                foreach (var scenario in summary)
                {
                    ResourceCriticalPath resource;
                    if (equipments)
                        resource = scenario.Equipments.FirstOrDefault(e => e.Label == resourceLabel);
                    else
                        resource = scenario.Operators.FirstOrDefault(e => e.Label == resourceLabel);

                    columnIndex++;

                    if (resource != null)
                    {
                        durationLine[columnIndex] = new CellContent(timeFormatService.TicksToString(resource.Duration)) { HorizontalAlignment = HorizontalAlignment.Center };
                        if (resource.EarningPercent.HasValue)
                            earningLine[columnIndex] = new CellContent(string.Format("{0:F}%", resource.EarningPercent)) { HorizontalAlignment = HorizontalAlignment.Center };
                        loadLine[columnIndex] = new CellContent(string.Format("{0:F}%", resource.LoadPercent)) { HorizontalAlignment = HorizontalAlignment.Center };
                        overloadLine[columnIndex] = new CellContent(string.Format("{0:F}%", resource.OverloadPercent)) { HorizontalAlignment = HorizontalAlignment.Center };
                        valuesLine[columnIndex] = new CellContent(string.Format(LocalizationManager.GetString("View_Shared_Summary_Values_StringFormat"),
                            resource.Values[KnownActionCategoryValues.VA],
                            resource.Values[KnownActionCategoryValues.BNVA],
                            resource.Values[KnownActionCategoryValues.NVA],
                            resource.Values[ScenarioCriticalPath.ValueNoneKey]
                            )) { HorizontalAlignment = HorizontalAlignment.Center };
                    }
                }

                rows[rowIndex] = durationLine;
                rowIndex++;
                rows[rowIndex] = earningLine;
                rowIndex++;
                rows[rowIndex] = loadLine;
                rowIndex++;
                rows[rowIndex] = overloadLine;
                rowIndex++;
                rows[rowIndex] = valuesLine;
                rowIndex++;

            }

            dataGrid.ItemsSource = rows;
        }

        /// <summary>
        /// Crée un DataTemplate pour représenter les données.
        /// </summary>
        /// <param name="index">L'index de colonne.</param>
        /// <param name="cellTemplate">La clé de la template de la colonne.</param>
        /// <returns>Le DataTemplate créé.</returns>
        private static DataTemplate CreateSummaryDataTemplate(int index, string cellTemplate)
        {
            string templateXaml =
                @"<DataTemplate 
                                        xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
                                        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                        <ContentControl x:Name='LayoutRoot' Content='{Binding %%BINDING%%}' ContentTemplate='{DynamicResource %%CellTemplateResourceKey%%}' />
                                    </DataTemplate>";

            templateXaml = templateXaml.Replace("%%BINDING%%", string.Format("[{0}]", index));
            templateXaml = templateXaml.Replace("%%CellTemplateResourceKey%%", cellTemplate);

            DataTemplate template;
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(templateXaml)))
            {
                template = (DataTemplate)XamlReader.Load(stream);
            }

            return template;
        }

        /// <summary>
        /// Ajoute une colonne avec les styles par défaut.
        /// </summary>
        /// <param name="columns">La collection des colonnes.</param>
        /// <param name="columnIndex">L'index de la colonne.</param>
        /// <param name="header">L'entête.</param>
        /// <param name="usetextContent"><c>true</c> pour utiliser le TextContent de la CellContent plutôt que le Content.</param>
        private static void AddColumn(ObservableCollection<DataGridColumn> columns, int columnIndex, object header = null, bool usetextContent = false)
        {
            var column = new DataGridTemplateColumn()
            {
                Header = header,
                CellTemplate = CreateSummaryDataTemplate(columnIndex, "SummaryDefaultCellTemplate"),
            };
            columns.Add(column);
            Interaction.GetBehaviors(column).Add(new ExportFormatBehavior()
            {
                Header = header != null ? header.ToString() : null,
                Binding = CreateContentBinding(columnIndex, usetextContent ? "TextContent" : "Content"),
                CellDataType = Core.ExcelExport.CellDataType.String,
            });
        }

        /// <summary>
        /// Crée un binding permettant d'accéder à la donnée finale.
        /// </summary>
        /// <param name="index">L'index de la données.</param>
        /// <returns>
        /// Le binding
        /// </returns>
        private static Binding CreateContentBinding(int index, string path)
        {
            return new Binding(string.Format("[{0}].{1}", index, path));
        }

        /// <summary>
        /// Représente le contenu d'une cellule.
        /// </summary>
        public class CellContent
        {
            /// <summary>
            /// Initialise une nouvelle instance de la classe <see cref="CellContent"/>.
            /// </summary>
            /// <param name="content">Le contenu.</param>
            public CellContent(object content)
            {
                this.Content = content;
                this.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                this.FontWeight = FontWeights.Normal;
                this.TextContent = content != null ? content.ToString() : null;
            }

            /// <summary>
            /// Obtient ou définit le contenu.
            /// </summary>
            public object Content { get; set; }

            public FontWeight FontWeight { get; set; }

            public HorizontalAlignment HorizontalAlignment { get; set; }

            public string TextContent { get; set; }

            public static implicit operator CellContent(string s)
            {
                return new CellContent(s);
            }
        }
    }
}
