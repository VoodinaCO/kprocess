using DocumentFormat.OpenXml.Packaging;
using KProcess.Globalization;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Business.Referentials;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Core.ExcelExport;
using KProcess.Ksmed.Presentation.Shell.Views;
using KProcess.Ksmed.Security;
using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.ViewModels.Restitution
{
    /// <summary>
    /// Gère l'export d'un projet vers Excel.
    /// </summary>
    internal class ExportProjectToExcel
    {

        private Business.Dtos.RestitutionData _data;
        private ExportResult _export;
        private ExcelExporter _file;
        private Dictionary<ProcessReferentialIdentifier, ProjectReferential> _referentialsUse;

        public ExportProjectToExcel(Business.Dtos.RestitutionData data, ExportResult export)
        {
            _data = data;
            _export = export;
        }


        /// <summary>
        /// Exporte les données spécifiées dans le fichier spécifié.
        /// </summary>
        /// <param name="data">Les données.</param>
        /// <param name="export">Les informations d'export.</param>
        public async Task Export()
        {
            var fileName = ExcelExporter.GetFileNameWithExtension(_export.Filename);
            try
            {
                _file = await ExcelExporter.Create(fileName);
            }
            catch (ExcelExporter.FileAlreadyInUseExeption)
            {
                return; // Une notification a déjà été levée dans ce cas
            }

            var projectSheet = _file.CreateSheet(LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Project"));
            ExportProject(projectSheet);

            var videosSheet = _file.CreateSheet(LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Videos"));
            var videoCellReference = new CellReference();
            foreach (var video in _data.Project.Process.Videos)
                ExportVideo(video, videosSheet, videoCellReference);

            var referentialsSheet = _file.CreateSheet(LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Referentials"));
            ExportReferentials(_data.Project, referentialsSheet);

            var membersSheet = _file.CreateSheet(LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Members"));
            ExportMembers(_data.Project, membersSheet);

            _referentialsUse = _data.Project.Referentials.ToDictionary(pr => (ProcessReferentialIdentifier)pr.ReferentialId, pr => pr);

            foreach (var scenario in _data.Project.Scenarios)
            {
                var scenarioSheet = _file.CreateSheet(scenario.Label);
                ExportScenario(scenario, scenarioSheet);
            }

            _file.SaveAndClose();

            if (_export.OpenWhenCreated)
            {
                System.Diagnostics.Process.Start(fileName);
            }
        }

        /// <summary>
        /// Exporte le projet.
        /// </summary>
        /// <param name="sheet">La feuille.</param>
        private void ExportProject(WorksheetPart sheet)
        {
            var project = _data.Project;
            var cellReference = new CellReference();

            // Nom
            _file.SetCellValue(sheet, cellReference, project.Label);
            cellReference.NewLine();

            // Description
            _file.SetCellValue(sheet, cellReference, project.Description);
            cellReference.NewLine();

            // Utilisateur
            _file.SetCellValue(sheet, cellReference, SecurityContext.CurrentUser.FullName);
            cellReference.NewLine();

            // Objectif
            SetLabelValue(sheet, cellReference, "ViewModel_AnalyzeRestitution_Export_Project_Objective",
                project.Objective != null ? project.Objective.LongLabel : project.OtherObjectiveLabel);

            // Atelier
            SetLabelValue(sheet, cellReference, "ViewModel_AnalyzeRestitution_Export_Project_Workshop", project.Workshop);

            // Custom Labels
            SetLabelValue(sheet, cellReference, "ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text1", project.CustomTextLabel);
            SetLabelValue(sheet, cellReference, "ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text2", project.CustomTextLabel2);
            SetLabelValue(sheet, cellReference, "ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text3", project.CustomTextLabel3);
            SetLabelValue(sheet, cellReference, "ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Text4", project.CustomTextLabel4);
            SetLabelValue(sheet, cellReference, "ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric1", project.CustomNumericLabel);
            SetLabelValue(sheet, cellReference, "ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric2", project.CustomNumericLabel2);
            SetLabelValue(sheet, cellReference, "ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric3", project.CustomNumericLabel3);
            SetLabelValue(sheet, cellReference, "ViewModel_AnalyzeRestitution_Export_Project_CustomLabel_Numeric4", project.CustomNumericLabel4);

            // Version
            SetLabelValue(sheet, cellReference, "ViewModel_AnalyzeRestitution_Export_Project_ApplicationVersion", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

            // Precision
            SetLabelValue(sheet, cellReference, "ViewModel_AnalyzeRestitution_Export_Project_Precision", (int)TimeSpan.FromTicks(project.TimeScale).TotalMilliseconds);


            // Summary
            /////////////////////////
            cellReference.NewLine();
            cellReference.NewLine();


            var dataGrid = new System.Windows.Controls.DataGrid();
            SummaryBuilder.BuildScenarios(project.ScenariosCriticalPath, dataGrid);

            var headers = dataGrid.Columns.Select(column => column.Header);
            if (dataGrid.ItemsSource is object[][] dataSource)
            {
                foreach (var header in headers)
                {
                    _file.SetCellValue(sheet, cellReference, header.SafeToString());
                    cellReference.MoveRight();
                }

                cellReference.NewLine();
                foreach (var row in dataSource)
                {
                    foreach (var cell in row)
                    {
                        if (cell != null)
                        {
                            if (cell is SummaryBuilder.CellContent)
                            {
                                _file.SetCellValue(sheet, cellReference, ((SummaryBuilder.CellContent)cell).Content.SafeToString());
                            }
                            else
                            {
                                _file.SetCellValue(sheet, cellReference, cell.ToString());
                            }
                        }
                        cellReference.MoveRight();
                    }
                    cellReference.NewLine();
                }
            }
        }

        /// <summary>
        /// Exporte la vidéo spécifiée.
        /// </summary>
        /// <param name="video">La vidéo.</param>
        /// <param name="sheet">La feuille.</param>
        /// <param name="cellReference">La référence de la cellule où commencer l'export.</param>
        private void ExportVideo(Video video, WorksheetPart sheet, CellReference cellReference)
        {
            // Nom
            SetLabelValue(sheet, cellReference, "View_PrepareVideos_Name", video.Filename);

            // Fichier
            SetLabelValue(sheet, cellReference, "View_PrepareVideos_File", video.FilePath);

            // Durée
            SetLabelValue(sheet, cellReference, "ViewModel_AnalyzeRestitution_Export_Video_Duration", CellContent.TimeSpan((long)video.Duration));

            // Date de prise de vue
            SetLabelValue(sheet, cellReference, "View_PrepareVideos_ShootingDate", video.ShootingDate);

            // Description
            //SetLabelValue(sheet, cellReference, "View_PrepareVideos_Description", video.Description);

            // POV/VSM
            _file.SetCellValue(sheet, cellReference, LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Video_POV"));
            cellReference.MoveRight();

            if (video.ResourceView != null)
            {
                _file.SetCellValue(sheet, cellReference, LocalizationManager.GetString("VideoNaure_POV_Short"));
                cellReference.NewLine();

                //  Opérateur/Equipement + Nom
                _file.SetCellValue(sheet, cellReference, LocalizationManager.GetString("View_PrepareVideos_DefaultResource"));
                cellReference.MoveRight();
                _file.SetCellValue(sheet, cellReference, video.DefaultResource.Label);
                cellReference.NewLine();
            }
            else
            {
                _file.SetCellValue(sheet, cellReference, LocalizationManager.GetString("VideoNaure_VSM_Short"));
                cellReference.NewLine();
            }

            cellReference.NewLine();

        }

        /// <summary>
        /// Exporte les membres utilisés.
        /// </summary>
        /// <param name="project">Le projet.</param>
        /// <param name="sheet">La feuille.</param>
        private void ExportMembers(Project project, WorksheetPart sheet)
        {
            //var prepareService = IoC.Resolve<IPrepareService>().GetMembers()

            var cellReference = new CellReference();

            _file.SetCellValue(sheet, cellReference, LocalizationManager.GetString("View_ApplicationMembers_Username"));
            cellReference.MoveRight();

            _file.SetCellValue(sheet, cellReference, LocalizationManager.GetString("View_ApplicationMembers_Firstname"));
            cellReference.MoveRight();

            _file.SetCellValue(sheet, cellReference, LocalizationManager.GetString("View_ApplicationMembers_Name"));
            cellReference.MoveRight();

            _file.SetCellValue(sheet, cellReference, LocalizationManager.GetString("View_ApplicationMembers_Email"));
            cellReference.MoveRight();

            _file.SetCellValue(sheet, cellReference, LocalizationManager.GetString("View_ApplicationMembers_PhoneNumber"));
            cellReference.MoveRight();

            _file.SetCellValue(sheet, cellReference, LocalizationManager.GetString("View_ApplicationMembers_DefaultLanguage"));
            cellReference.MoveRight();

            _file.SetCellValue(sheet, cellReference, LocalizationManager.GetString("View_ApplicationMembers_Roles"));
            cellReference.NewLine();

            foreach (var userInRoles in project?.Process.UserRoleProcesses.GroupBy(ur => ur.User))
            {
                var user = userInRoles.Key;
                _file.SetCellValue(sheet, cellReference, user.Username);
                cellReference.MoveRight();

                _file.SetCellValue(sheet, cellReference, user.Firstname);
                cellReference.MoveRight();

                _file.SetCellValue(sheet, cellReference, user.Name);
                cellReference.MoveRight();

                _file.SetCellValue(sheet, cellReference, user.Email);
                cellReference.MoveRight();

                _file.SetCellValue(sheet, cellReference, user.PhoneNumber);
                cellReference.MoveRight();

                _file.SetCellValue(sheet, cellReference, user.DefaultLanguage?.Label);
                cellReference.MoveRight();

                _file.SetCellValue(sheet, cellReference, string.Join(";", userInRoles.Select(uir => uir.RoleCode)));
                cellReference.NewLine();
            }
        }

        /// <summary>
        /// Exporte les référentiels utilisés.
        /// </summary>
        /// <param name="project">Le projet.</param>
        /// <param name="sheet">La feuille.</param>
        private void ExportReferentials(Project project, WorksheetPart sheet)
        {
            var cellReference = new CellReference();

            var allReferentials = ReferentialsHelper.GetAllReferentialsStandardUsed(project).Cast<IActionReferential>().Concat(ReferentialsHelper.GetAllReferentialsProject(project)).ToArray();

            var refUseService = IoC.Resolve<IReferentialsUseService>();

            foreach (var pr in project.Referentials.Where(r => r.IsEnabled))
            {
                // Nom
                _file.SetCellValue(sheet, cellReference, refUseService.GetLabel((ProcessReferentialIdentifier)pr.ReferentialId));
                cellReference.NewLine();

                IEnumerable<IActionReferential> referentials;

                switch ((ProcessReferentialIdentifier)pr.ReferentialId)
                {
                    case ProcessReferentialIdentifier.Operator:
                        referentials = allReferentials.OfType<Operator>();
                        break;
                    case ProcessReferentialIdentifier.Equipment:
                        referentials = allReferentials.OfType<Equipment>();
                        break;
                    case ProcessReferentialIdentifier.Category:
                        referentials = allReferentials.OfType<ActionCategory>();
                        break;
                    case ProcessReferentialIdentifier.Ref1:
                        referentials = allReferentials.OfType<Ref1>();
                        break;
                    case ProcessReferentialIdentifier.Ref2:
                        referentials = allReferentials.OfType<Ref2>();
                        break;
                    case ProcessReferentialIdentifier.Ref3:
                        referentials = allReferentials.OfType<Ref3>();
                        break;
                    case ProcessReferentialIdentifier.Ref4:
                        referentials = allReferentials.OfType<Ref4>();
                        break;
                    case ProcessReferentialIdentifier.Ref5:
                        referentials = allReferentials.OfType<Ref5>();
                        break;
                    case ProcessReferentialIdentifier.Ref6:
                        referentials = allReferentials.OfType<Ref6>();
                        break;
                    case ProcessReferentialIdentifier.Ref7:
                        referentials = allReferentials.OfType<Ref7>();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var referential in referentials)
                {
                    _file.SetCellValue(sheet, cellReference, referential.Label);

                    cellReference.MoveRight();
                    if (referential.CloudFile != null)
                        _file.SetCellValue(sheet, cellReference, referential.CloudFile.LocalUri.AbsoluteUri);

                    cellReference.MoveRight();
                    if (!string.IsNullOrWhiteSpace(referential.Description))
                    {
                        _file.SetCellValue(sheet, cellReference, referential.Description);
                    }

                    cellReference.NewLine();
                }

                cellReference.NewLine();
            }
        }

        /// <summary>
        /// Exporte le scénario spécifié.
        /// </summary>
        /// <param name="scenario">Le scénario.</param>
        /// <param name="sheet">La feuille.</param>
        private void ExportScenario(Scenario scenario, WorksheetPart sheet)
        {
            var cellReference = new CellReference();

            // Nom
            _file.SetCellValue(sheet, cellReference, scenario.Label);
            cellReference.NewLine();

            // Description
            _file.SetCellValue(sheet, cellReference, scenario.Description);
            cellReference.NewLine();
            cellReference.NewLine();

            // Nature
            _file.SetCellValue(sheet, cellReference, scenario.Nature.LongLabel);
            cellReference.NewLine();

            // Etat
            _file.SetCellValue(sheet, cellReference, scenario.State.LongLabel);
            cellReference.NewLine();

            if (scenario.Original != null)
            {
                // Original
                _file.SetCellValue(sheet, cellReference, scenario.Original.Label);
                cellReference.MoveRight();
                _file.SetCellValue(sheet, cellReference, scenario.Original.State.LongLabel);
                cellReference.NewLine();
            }
            cellReference.NewLine();

            var actions = scenario.Actions.OrderByWBS().ToList();

            ExportActions(actions, sheet, ref cellReference);
            cellReference.NewLine();
            cellReference.NewLine();

            ExportSolutions(scenario, cellReference, sheet);

        }

        /// <summary>
        /// Exporte les actions spécifiées.
        /// </summary>
        /// <param name="actions">Les actions.</param>
        /// <param name="cellReference">La référence de la cellule.</param>
        /// <param name="sheet">La feuille.</param>
        private void ExportActions(ICollection<KAction> actions, WorksheetPart sheet, ref CellReference cellReference)
        {
            var timeFormatService = IoC.Resolve<IServiceBus>().Get<ITimeTicksFormatService>();

            var timeScale = _data.Project.TimeScale;

            #region Format pour les actions

            // Actions
            var actionsFormats = new List<ColumnFormat>()
            {


                // Thumbnail
                new ColumnFormat() { Header = string.Empty },

                // Label
                new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_Task") },

                // Start
                new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_Start") },

                // Duration
                new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_Duration") },

                // Finish
                new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_Finish") },

                // BuildStart
                new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_BuildStart") },

                // BuildDuration
                new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_BuildDuration") },

                // BuildFinish
                new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_BuildFinish") },

                // WBS
                new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_WBS") },

                // Category
                new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_Category") },

                // Resource
                new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_Resource") },

                // Video
                new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_Video") },

                // Predecessors
                new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_Predecessors") },

                // Original
                new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_Original") },

            };

            // Ref1
            if (_referentialsUse[ProcessReferentialIdentifier.Ref1].IsEnabled)
                actionsFormats.Add(new ColumnFormat() { Header = IoC.Resolve<IReferentialsUseService>().GetLabel(ProcessReferentialIdentifier.Ref1) });

            // Ref2
            if (_referentialsUse[ProcessReferentialIdentifier.Ref2].IsEnabled)
                actionsFormats.Add(new ColumnFormat() { Header = IoC.Resolve<IReferentialsUseService>().GetLabel(ProcessReferentialIdentifier.Ref2) });

            // Ref3
            if (_referentialsUse[ProcessReferentialIdentifier.Ref3].IsEnabled)
                actionsFormats.Add(new ColumnFormat() { Header = IoC.Resolve<IReferentialsUseService>().GetLabel(ProcessReferentialIdentifier.Ref3) });

            // Ref4
            if (_referentialsUse[ProcessReferentialIdentifier.Ref4].IsEnabled)
                actionsFormats.Add(new ColumnFormat() { Header = IoC.Resolve<IReferentialsUseService>().GetLabel(ProcessReferentialIdentifier.Ref4) });

            // Ref5
            if (_referentialsUse[ProcessReferentialIdentifier.Ref5].IsEnabled)
                actionsFormats.Add(new ColumnFormat() { Header = IoC.Resolve<IReferentialsUseService>().GetLabel(ProcessReferentialIdentifier.Ref5) });

            // Ref6
            if (_referentialsUse[ProcessReferentialIdentifier.Ref6].IsEnabled)
                actionsFormats.Add(new ColumnFormat() { Header = IoC.Resolve<IReferentialsUseService>().GetLabel(ProcessReferentialIdentifier.Ref6) });

            // Ref7
            if (_referentialsUse[ProcessReferentialIdentifier.Ref7].IsEnabled)
                actionsFormats.Add(new ColumnFormat() { Header = IoC.Resolve<IReferentialsUseService>().GetLabel(ProcessReferentialIdentifier.Ref7) });

            // IsRandom
            actionsFormats.Add(new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_IsRandom") });

            // Custom Text
            actionsFormats.Add(new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text1") });
            actionsFormats.Add(new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text2") });
            actionsFormats.Add(new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text3") });
            actionsFormats.Add(new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Text4") });

            // Custom Numeric
            actionsFormats.Add(new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric1") });
            actionsFormats.Add(new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric2") });
            actionsFormats.Add(new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric3") });
            actionsFormats.Add(new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_CustomValue_Numeric4") });

            // DifferenceReason
            actionsFormats.Add(new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_DifferenceReason") });

            // Amélioration I/E/S
            actionsFormats.Add(new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_Reduced_IES") });

            // Solution
            actionsFormats.Add(new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_Reduced_Solution") });

            // Reduction ratio
            actionsFormats.Add(new ColumnFormat() { Header = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_Reduced_ReductionRatio") });

            #endregion

            CellContent[][] data = new CellContent[actions.Count][];

            uint tableRowIndex = cellReference.RowIndex;

            int i = 0;
            foreach (var action in actions)
            {
                CellContent[] row = new CellContent[actionsFormats.Count];

                // Mettre à jour IsGroup
                action.IsGroup = WBSHelper.HasChildren(action, actions);

                int j = 0;

                #region Data pour les actions

                // Thumbnail
                j++;

                // Label
                row[j++] = action.Label;

                if (!action.IsGroup)
                {
                    // Start
                    row[j++] = CellContent.TimeSpan(timeFormatService.RoundTime(action.Start, timeScale));

                    // Duration
                    row[j++] = CellContent.TimeSpan(timeFormatService.RoundTime(action.Duration, timeScale));

                    // Finish
                    row[j++] = CellContent.TimeSpan(timeFormatService.RoundTime(action.Finish, timeScale));
                }
                else
                    j += 3; // Nombre de colonnes précédentes

                // BuildStart
                row[j++] = CellContent.TimeSpan(timeFormatService.RoundTime(action.BuildStart, timeScale));

                // BuildDuration
                row[j++] = CellContent.TimeSpan(timeFormatService.RoundTime(action.BuildDuration, timeScale));

                // BuildFinish
                row[j++] = CellContent.TimeSpan(timeFormatService.RoundTime(action.BuildFinish, timeScale));

                // WBS
                row[j++] = action.WBS;


                if (!action.IsGroup)
                {
                    // Category
                    if (action.Category != null)
                        row[j++] = action.Category.Label;
                    else
                        row[j++] = null;

                    // Resource
                    if (action.Resource != null)
                        row[j++] = action.Resource.Label;
                    else
                        row[j++] = null;

                    // Video
                    if (action.Video != null)
                        row[j++] = action.Video.Filename;
                    else
                        row[j++] = null;

                    // Predecessors
                    row[j++] = FormatPredecessorsString(action);

                    // Original
                    if (action.Original != null)
                        row[j++] = action.Original.Label;
                    else
                        row[j++] = null;

                    // Ref1
                    if (_referentialsUse[ProcessReferentialIdentifier.Ref1].IsEnabled)
                        row[j++] = GetMultiReferentialLabels(action.Ref1, ProcessReferentialIdentifier.Ref1);

                    // Ref2
                    if (_referentialsUse[ProcessReferentialIdentifier.Ref2].IsEnabled)
                        row[j++] = GetMultiReferentialLabels(action.Ref2, ProcessReferentialIdentifier.Ref2);

                    // Ref3
                    if (_referentialsUse[ProcessReferentialIdentifier.Ref3].IsEnabled)
                        row[j++] = GetMultiReferentialLabels(action.Ref3, ProcessReferentialIdentifier.Ref3);

                    // Ref4
                    if (_referentialsUse[ProcessReferentialIdentifier.Ref4].IsEnabled)
                        row[j++] = GetMultiReferentialLabels(action.Ref4, ProcessReferentialIdentifier.Ref4);

                    // Ref5
                    if (_referentialsUse[ProcessReferentialIdentifier.Ref5].IsEnabled)
                        row[j++] = GetMultiReferentialLabels(action.Ref5, ProcessReferentialIdentifier.Ref5);

                    // Ref6
                    if (_referentialsUse[ProcessReferentialIdentifier.Ref6].IsEnabled)
                        row[j++] = GetMultiReferentialLabels(action.Ref6, ProcessReferentialIdentifier.Ref6);

                    // Ref7
                    if (_referentialsUse[ProcessReferentialIdentifier.Ref7].IsEnabled)
                        row[j++] = GetMultiReferentialLabels(action.Ref7, ProcessReferentialIdentifier.Ref7);

                    // IsRandom
                    row[j++] = action.IsRandom.ToString();

                    // Custom Text
                    row[j++] = action.CustomTextValue;
                    row[j++] = action.CustomTextValue2;
                    row[j++] = action.CustomTextValue3;
                    row[j++] = action.CustomTextValue4;

                    // Custom Numeric
                    row[j++] = action.CustomNumericValue;
                    row[j++] = action.CustomNumericValue2;
                    row[j++] = action.CustomNumericValue3;
                    row[j++] = action.CustomNumericValue4;

                    // DifferenceReason
                    row[j++] = action.DifferenceReason;

                    // Amélioration
                    if (action.Reduced != null)
                    {
                        // Amélioration I/E/S
                        string label;

                        if (ActionsTimingsMoveManagement.IsActionInternal(action))
                            label = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_Reduced_Internal");
                        else if (ActionsTimingsMoveManagement.IsActionExternal(action))
                            label = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_Reduced_External");
                        else if (ActionsTimingsMoveManagement.IsActionDeleted(action))
                            label = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_Reduced_Deleted");
                        else
                            throw new ArgumentOutOfRangeException();

                        row[j++] = label;

                        row[j++] = action.Reduced.Solution;
                        row[j++] = CellContent.Percentage(action.Reduced.ReductionRatio);
                    }
                    else
                    {
                        row[j++] = null;
                        row[j++] = null;
                        row[j++] = null;
                    }
                }

                #endregion

                data[i] = row;
                i++;
            }

            _file.AddTable(sheet, actionsFormats.ToArray(), data, cellReference);


            // Ajouter une image

            cellReference.NewLine();

            i = 0;
            foreach (var action in actions)
            {
                if (action.Thumbnail != null)
                {
                    // Ajout du libellé du lien
                    var tableCellRef = new CellReference(1, tableRowIndex + (uint)i + 1);
                    _file.SetCellValue(sheet, tableCellRef, new CellContent(action.ActionId.ToString(), CellDataType.Hyperlink));

                    // Ajout du lien
                    string definedName = string.Format("Action.{0}", action.ActionId);
                    _file.CreateDefinedName(sheet, cellReference, definedName);

                    _file.AddHyperlinkToDefinedName(sheet, tableCellRef, definedName, "");

                    // Ajout du libellé
                    _file.SetCellValue(sheet, cellReference, string.Format("{0} {1}", action.WBS, action.Label ?? string.Empty));
                    cellReference.MoveRight();

                    // Ajout de l'image
                    using (var ms = new System.IO.MemoryStream(action.Thumbnail.Data))
                    {
                        var decoder = System.Windows.Media.Imaging.BitmapDecoder.Create(ms, System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat, System.Windows.Media.Imaging.BitmapCacheOption.OnLoad);
                        var size = new System.Windows.Size(decoder.Frames[0].PixelWidth, decoder.Frames[0].PixelHeight);
                        var mimeTypes = decoder.CodecInfo.MimeTypes;

                        ImagePartType imageType;
                        if (mimeTypes.Contains("image/jpeg"))
                        {
                            imageType = ImagePartType.Jpeg;
                        }
                        else if (mimeTypes.Contains("image/bmp"))
                        {
                            imageType = ImagePartType.Bmp;
                        }
                        else if (mimeTypes.Contains("image/png"))
                        {
                            imageType = ImagePartType.Png;
                        }
                        else
                            continue;

                        string pictureName = string.Format("Thumbnail.{0}", action.ActionId);

                        uint rowsLength = _file.AddImage(sheet, action.Thumbnail.Data, imageType, action.ActionId.ToString(), pictureName, size, cellReference);

                        // On déplace la cellule active à après l'image
                        cellReference = new CellReference(1, cellReference.RowIndex + rowsLength);
                    }
                }
                i++;
            }
        }

        /// <summary>
        /// Exporte une paire libellé - valeur.
        /// </summary>
        /// <param name="sheet">La feuille.</param>
        /// <param name="cellReference">La référence vers la cellule.</param>
        /// <param name="labelGlobalizationKey">Le clé de globalisation du libellé.</param>
        /// <param name="content">Le contenu.</param>
        private void SetLabelValue(WorksheetPart sheet, CellReference cellReference, string labelGlobalizationKey, CellContent content)
        {
            _file.SetCellValue(sheet, cellReference, LocalizationManager.GetString(labelGlobalizationKey));
            cellReference.MoveRight();
            _file.SetCellValue(sheet, cellReference, content);
            cellReference.NewLine();
        }

        /// <summary>
        /// Obtient les libellés des référentiels de l'action concaténés, en fonction de leur utilisation et de leurs options.
        /// </summary>
        /// <param name="links">Les liens Référentiel - Action.</param>
        /// <param name="refeId">L'identifiant de chaque référentiel utilisé.</param>
        /// <returns>La chaîne concaténée.</returns>
        private string GetMultiReferentialLabels(IEnumerable<IReferentialActionLink> links, ProcessReferentialIdentifier refeId)
        {
            if (_referentialsUse[refeId].IsEnabled)
            {
                var useQuantity = _referentialsUse[refeId].HasQuantity;
                IEnumerable<string> values;
                if (useQuantity)
                    values = links.Select(al => string.Format(LocalizationManager.GetString("Common_Referentials_QuantityDescription"), al.Quantity, al.Referential.Label));
                else
                    values = links.Select(al => al.Referential.Label);
                return string.Join(Environment.NewLine, values);
            }
            else
                return null;
        }

        /// <summary>
        /// Retourne la liste des prédécesseurs sous forme de chaîne formattée.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <returns>Les prédécesseurs</returns>
        private static string FormatPredecessorsString(KAction action)
        {
            if (!action.Predecessors.Any())
                return string.Empty;
            else
                return string.Join(", ", action.Predecessors.Select(a => a.WBS));
        }

        /// <summary>
        /// Exporte les solutions.
        /// </summary>
        /// <param name="scenario">Le scénario.</param>
        /// <param name="cellReference">La référence de la cellule.</param>
        /// <param name="file">Le fichier.</param>
        /// <param name="sheet">La feuille.</param>
        private void ExportSolutions(Scenario scenario, CellReference cellReference, WorksheetPart sheet)
        {
            ITimeTicksFormatService timeFormatService = IoC.Resolve<IServiceBus>().Get<ITimeTicksFormatService>();
            long timeScale = _data.Project.TimeScale;

            List<SolutionWrapper> solutions = new List<SolutionWrapper>();

            foreach (var solution in scenario.Solutions.OrderBy(s => s.SolutionDescription))
            {
                var w = CreateSolutionWrapper(scenario.Actions, solution);
                solutions.Add(w);
            }

            var originalScenario = scenario.Original;
            while (originalScenario != null)
            {
                // Déterminer les actions qui sont concernées
                var originalActions = originalScenario.Actions.Where(originalAction =>
                    scenario.Actions.Any(currentScenarioAction =>
                        ScenarioActionHierarchyHelper.IsAncestor(originalAction, currentScenarioAction)));

                foreach (var solution in originalScenario.Solutions.OrderBy(s => s.SolutionDescription))
                {
                    var wrapper = CreateSolutionWrapper(originalActions, solution);
                    // Ignorer les solutions qui n'apportent pas de gain. C'est un surplus d'infos inutile
                    if (wrapper.Saving != 0)
                        solutions.Add(wrapper);
                }

                originalScenario = originalScenario.Original;
            }

            // Définir l'index
            int i = 1;
            foreach (var wrapper in solutions)
                wrapper.Index = i++;

            var solutionsFormats = new ColumnFormat[]
            {
                #region Format pour les solutions

                // Index
                new ColumnFormat() { Header = "" },

                // Scénario
                new ColumnFormat() { Header = LocalizationManager.GetString("View_RestitutionSolutions_Columns_Scenario") },

                // Solution
                new ColumnFormat() { Header = LocalizationManager.GetString("View_RestitutionSolutions_Columns_Solution") },

                // Tâches liées
                new ColumnFormat() { Header = LocalizationManager.GetString("View_RestitutionSolutions_Columns_RelatedActions") },

                // Gain
                new ColumnFormat() { Header = LocalizationManager.GetString("View_RestitutionSolutions_Columns_Saving") },

                // Investissement
                new ColumnFormat() { Header = LocalizationManager.GetString("View_RestitutionSolutions_Columns_Investment") },

                // I/G
                new ColumnFormat() { Header = LocalizationManager.GetString("View_RestitutionSolutions_Columns_IG") },

                // Difficulté
                new ColumnFormat() { Header = LocalizationManager.GetString("View_RestitutionSolutions_Columns_Diffculty") },

                // Coûts
                new ColumnFormat() { Header = LocalizationManager.GetString("View_RestitutionSolutions_Columns_Cost") },

                // DC/G
                new ColumnFormat() { Header = LocalizationManager.GetString("View_RestitutionSolutions_Columns_DCG") },

                // OK
                new ColumnFormat() { Header = LocalizationManager.GetString("View_RestitutionSolutions_Columns_OK") },

                // Comments
                new ColumnFormat() { Header = LocalizationManager.GetString("View_RestitutionSolutions_Columns_Comments") },

                #endregion
            };

            CellContent[][] data = new CellContent[solutions.Count][];

            i = 0;
            foreach (var solution in solutions)
            {
                CellContent[] row = new CellContent[solutionsFormats.Length];

                int j = 0;

                #region Data

                // Index
                row[j++] = solution.Index;

                // Scénario
                row[j++] = solution.Solution.Scenario.Label;

                // Solution
                row[j++] = solution.Solution.SolutionDescription;

                // Tâches liées
                row[j++] = solution.RelatedActions;

                // Gain
                row[j++] = CellContent.TimeSpan(timeFormatService.RoundTime(solution.Saving, timeScale));

                // Investissement
                if (solution.Solution.Investment.HasValue)
                    row[j++] = solution.Solution.Investment;
                else
                    row[j++] = null;

                // I/G
                if (solution.IG.HasValue)
                    row[j++] = solution.IG;
                else
                    row[j++] = null;

                // Difficulté
                if (solution.Solution.Difficulty.HasValue)
                    row[j++] = solution.Solution.Difficulty;
                else
                    row[j++] = null;

                // Coûts
                if (solution.Solution.Cost.HasValue)
                    row[j++] = solution.Solution.Cost;
                else
                    row[j++] = null;

                // DC/G
                if (solution.DCG.HasValue)
                    row[j++] = solution.DCG;
                else
                    row[j++] = null;

                // OK
                row[j++] = solution.Solution.Approved.ToString();

                // Comments
                row[j++] = solution.Solution.Comments;

                #endregion

                data[i] = row;
                i++;
            }

            _file.AddTable(sheet, solutionsFormats, data, cellReference);
        }

        /// <summary>
        /// Crée un conteneur pour la solution.
        /// </summary>
        /// <param name="actions">Les actions qui peuvent être liées à la solution.</param>
        /// <param name="solution">La solution.</param>
        /// <returns>Le conteneur</returns>
        private static Restitution.SolutionWrapper CreateSolutionWrapper(IEnumerable<KAction> actions, Solution solution)
        {
            var wrapper = new Restitution.SolutionWrapper(solution);
            wrapper.SetRelatedActions(actions);

            return wrapper;
        }

    }
}
