using KProcess.KL2.JWT;
using KProcess.KL2.Languages;
using KProcess.KL2.WebAdmin.Models.Action;
using KProcess.KL2.WebAdmin.Models.Documentation;
using KProcess.KL2.WebAdmin.Models.DTO;
using KProcess.KL2.WebAdmin.Utils;
using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace KProcess.KL2.WebAdmin.Mapper
{
    public static class ActionMapper
    {
        static readonly List<string> ImageExtensions = new List<string> { ".png", ".jpg" };


        public static ActionColumnViewModel GetPublishedActionAttributes(
            PublishedAction action,
            object attribute,
            string header,
            string mapping,
            Dictionary<string, string> localizations,
            List<RefsCollection> refCollections,
            List<CustomLabel> customLabel
            )
        {
            var model = new ActionColumnViewModel { Values = new List<ActionValueViewModel>() };
            var referenceMappingNames = new string[] { "Refs1", "Refs2", "Refs3", "Refs4", "Refs5", "Refs6", "Refs7" };

            if (referenceMappingNames.Contains(mapping))
            {
                switch (mapping)
                {
                    case "Refs1":
                        model.Values = BuildPublishedReferentialFile(refCollections.FirstOrDefault(u => u.Label == localizations[nameof(Ref1)]));
                        break;
                    case "Refs2":
                        model.Values = BuildPublishedReferentialFile(refCollections.FirstOrDefault(u => u.Label == localizations[nameof(Ref2)]));
                        break;
                    case "Refs3":
                        model.Values = BuildPublishedReferentialFile(refCollections.FirstOrDefault(u => u.Label == localizations[nameof(Ref3)]));
                        break;
                    case "Refs4":
                        model.Values = BuildPublishedReferentialFile(refCollections.FirstOrDefault(u => u.Label == localizations[nameof(Ref4)]));
                        break;
                    case "Refs5":
                        model.Values = BuildPublishedReferentialFile(refCollections.FirstOrDefault(u => u.Label == localizations[nameof(Ref5)]));
                        break;
                    case "Refs6":
                        model.Values = BuildPublishedReferentialFile(refCollections.FirstOrDefault(u => u.Label == localizations[nameof(Ref6)]));
                        break;
                    case "Refs7":
                        model.Values = BuildPublishedReferentialFile(refCollections.FirstOrDefault(u => u.Label == localizations[nameof(Ref7)]));
                        break;
                }
                return model;
            }

            switch (mapping)
            {   
                case nameof(PublishedAction.Thumbnail):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Image",
                        Value = action.ThumbnailHash,
                        FileHash = action.ThumbnailHash,
                        FileExt = action.Thumbnail?.Extension
                    });
                    break;
                case nameof(ProcessReferentialIdentifier.Skill):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = attribute != null ? ((Skill)attribute).Label : ""
                    });
                    break;
                case nameof(ProcessReferentialIdentifier.Operator):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = attribute != null ? ((Operator)attribute).Label : ""
                    });
                    break;
                case nameof(ProcessReferentialIdentifier.Equipment):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = attribute != null ? ((Equipment)attribute).Label : ""
                    });
                    break;
                case nameof(PublishedAction.PublishedResource):
                    model.Values.Add(BuildPublishedResourceFile(action.PublishedResource));
                    break;
                case nameof(ProcessReferentialIdentifier.Category):
                case nameof(PublishedAction.PublishedActionCategory):
                    model.Values.Add(BuildPublishedActionCategoryFile(action.PublishedActionCategory));
                    break;
                case nameof(PublishedAction.Start):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = TicksUtil.TicksToString((long)attribute, action.Publication.TimeScale)
                    });
                    break;
                case nameof(PublishedAction.Finish):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = TicksUtil.TicksToString((long)attribute, action.Publication.TimeScale)
                    });
                    break;
                case nameof(PublishedAction.Duration):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = TicksUtil.TicksToString((long)attribute, action.Publication.TimeScale)
                    });
                    break;
                case nameof(PublishedAction.BuildStart):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = TicksUtil.TicksToString((long)attribute, action.Publication.TimeScale)
                    });
                    break;
                case nameof(PublishedAction.BuildFinish):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = TicksUtil.TicksToString((long)attribute, action.Publication.TimeScale)
                    });
                    break;
                case nameof(PublishedAction.Refs1):
                    model.Values = BuildPublishedReferentialFile(refCollections.FirstOrDefault(u => u.Label == localizations[nameof(Ref1)]));
                    break;
                case nameof(PublishedAction.Refs2):
                    model.Values = BuildPublishedReferentialFile(refCollections.FirstOrDefault(u => u.Label == localizations[nameof(Ref2)]));
                    break;
                case nameof(PublishedAction.Refs3):
                    model.Values = BuildPublishedReferentialFile(refCollections.FirstOrDefault(u => u.Label == localizations[nameof(Ref3)]));
                    break;
                case nameof(PublishedAction.Refs4):
                    model.Values = BuildPublishedReferentialFile(refCollections.FirstOrDefault(u => u.Label == localizations[nameof(Ref4)]));
                    break;
                case nameof(PublishedAction.Refs5):
                    model.Values = BuildPublishedReferentialFile(refCollections.FirstOrDefault(u => u.Label == localizations[nameof(Ref5)]));
                    break;
                case nameof(PublishedAction.Refs6):
                    model.Values = BuildPublishedReferentialFile(refCollections.FirstOrDefault(u => u.Label == localizations[nameof(Ref6)]));
                    break;
                case nameof(PublishedAction.Refs7):
                    model.Values = BuildPublishedReferentialFile(refCollections.FirstOrDefault(u => u.Label == localizations[nameof(Ref7)]));
                    break;
                case nameof(PublishedAction.CustomNumericValue):
                case nameof(PublishedAction.CustomNumericValue2):
                case nameof(PublishedAction.CustomNumericValue3):
                case nameof(PublishedAction.CustomNumericValue4):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = attribute != null ? Convert.ToDecimal(attribute).ToString(CultureInfo.CreateSpecificCulture(JwtTokenProvider.GetUserModelCurrentLanguage(System.Web.HttpContext.Current.Request.Cookies["token"].Value))).TrimEnd('0') : ""
                    });
                    break;
                default:
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = attribute != null ? attribute.ToString() : ""
                    });
                    break;
            }
            return model;
        }

        public static List<ActionValueViewModel> GetActionReferentials(List<RefsCollection> refs, string mapping, Dictionary<string, string> localizations)
        {
            var model = new List<ActionValueViewModel>();
            var localizeName = "";
            switch (mapping)
            {
                case "Refs1":
                    model = BuildPublishedReferentialFile(refs.FirstOrDefault(u => u.Label == localizations[nameof(Ref1)]));
                    localizeName = localizations[nameof(Ref1)];
                    break;
                case "Refs2":
                    model = BuildPublishedReferentialFile(refs.FirstOrDefault(u => u.Label == localizations[nameof(Ref2)]));
                    localizeName = localizations[nameof(Ref2)];
                    break;
                case "Refs3":
                    model = BuildPublishedReferentialFile(refs.FirstOrDefault(u => u.Label == localizations[nameof(Ref3)]));
                    localizeName = localizations[nameof(Ref3)];
                    break;
                case "Refs4":
                    model = BuildPublishedReferentialFile(refs.FirstOrDefault(u => u.Label == localizations[nameof(Ref4)]));
                    localizeName = localizations[nameof(Ref4)];
                    break;
                case "Refs5":
                    model = BuildPublishedReferentialFile(refs.FirstOrDefault(u => u.Label == localizations[nameof(Ref5)]));
                    localizeName = localizations[nameof(Ref5)];
                    break;
                case "Refs6":
                    model = BuildPublishedReferentialFile(refs.FirstOrDefault(u => u.Label == localizations[nameof(Ref6)]));
                    localizeName = localizations[nameof(Ref6)];
                    break;
                case "Refs7":
                    model = BuildPublishedReferentialFile(refs.FirstOrDefault(u => u.Label == localizations[nameof(Ref7)]));
                    localizeName = localizations[nameof(Ref7)];
                    break;
            }
            foreach (var item in model)
            {
                item.MappingName = mapping;
                item.LocalizeName = localizeName;
            }
            return model;
        }


        public static List<ActionValueViewModel> BuildPublishedReferentialFile(RefsCollection refs)
        {
            var values = new List<ActionValueViewModel>();

            if (refs == null)
                return values;

            foreach (var referential in refs.Values)
            {
                // Referential is only text
                if (referential.PublishedReferential.File == null)
                {
                    values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = referential.PublishedReferential.Label,
                        Quantity = referential.Quantity,
                        ReferentialId = referential.PublishedReferentialId
                    });
                }
                // Published file is an image
                else if (ImageExtensions.Contains(referential.PublishedReferential.File.Extension))
                {
                    values.Add(new ActionValueViewModel
                    {
                        Type = "Image",
                        Description = referential.PublishedReferential.Label,
                        Value = referential.PublishedReferential.File.Hash,
                        FileHash = referential.PublishedReferential.File.Hash,
                        FileExt = referential.PublishedReferential.File.Extension,
                        Quantity = referential.Quantity,
                        ReferentialId = referential.PublishedReferentialId
                    });
                }
                // Other type of file
                else
                {
                    values.Add(new ActionValueViewModel
                    {
                        Type = "File",
                        Description = referential.PublishedReferential.Label,
                        Value = referential.PublishedReferential.Label,
                        FileHash = referential.PublishedReferential.File.Hash,
                        FileExt = referential.PublishedReferential.File.Extension,
                        ReferentialId = referential.PublishedReferentialId
                    });
                }
            }
            return values;
        }

        public static ActionValueViewModel BuildPublishedResourceFile(PublishedResource pResource)
        {
            // Resource is only text
            if (pResource?.File == null)
                return new ActionValueViewModel
                {
                    Type = "Text",
                    Value = pResource?.Label ?? string.Empty
                };
            // Published file is an image
            if (ImageExtensions.Contains(pResource.File.Extension))
                return new ActionValueViewModel
                {
                    Type = "Image",
                    Description = pResource.Label,
                    Value = pResource.FileHash,
                    FileHash = pResource.FileHash,
                    FileExt = pResource.File.Extension
                };
            // Other type of file
            return new ActionValueViewModel
            {
                Type = "File",
                Description = pResource.Label,
                Value = pResource.Label ?? string.Empty,
                FileHash = pResource.FileHash,
                FileExt = pResource.File.Extension
            };
        }

        public static ActionValueViewModel BuildPublishedActionCategoryFile(PublishedActionCategory pCategory)
        {
            // Resource is only text
            if (pCategory?.File == null)
                return new ActionValueViewModel
                {
                    Type = "Text",
                    Value = pCategory?.Label ?? string.Empty
                };
            // Published file is an image
            if (ImageExtensions.Contains(pCategory.File.Extension))
                return new ActionValueViewModel
                {
                    Type = "Image",
                    Description = pCategory.Label,
                    Value = pCategory.FileHash,
                    FileHash = pCategory.FileHash,
                    FileExt = pCategory.File.Extension
                };
            // Other type of file
            return new ActionValueViewModel
            {
                Type = "File",
                Description = pCategory.Label,
                Value = pCategory.Label ?? string.Empty,
                FileHash = pCategory.FileHash,
                FileExt = pCategory.File.Extension
            };
        }

        public static Dictionary<string, string> GetColumnHeader(
            IEnumerable<SfDataGridGridColumn> visibleColumns,
            Dictionary<string, string> localizations)
        {
            var headers = new Dictionary<string, string>();
            foreach (var column in visibleColumns)
            {
                switch (column.HeaderText)
                {
                    case "Refs1":
                        headers.Add(column.MappingName, localizations[nameof(Ref1)]);
                        break;
                    case "Refs2":
                        headers.Add(column.MappingName, localizations[nameof(Ref2)]);
                        break;
                    case "Refs3":
                        headers.Add(column.MappingName, localizations[nameof(Ref3)]);
                        break;
                    case "Refs4":
                        headers.Add(column.MappingName, localizations[nameof(Ref4)]);
                        break;
                    case "Refs5":
                        headers.Add(column.MappingName, localizations[nameof(Ref5)]);
                        break;
                    case "Refs6":
                        headers.Add(column.MappingName, localizations[nameof(Ref6)]);
                        break;
                    case "Refs7":
                        headers.Add(column.MappingName, localizations[nameof(Ref7)]);
                        break;

                    case "CustomNumericValue":
                        headers.Add(column.MappingName, localizations["CustomNumericLabel"]);
                        break;
                    case "CustomNumericValue2":
                        headers.Add(column.MappingName, localizations["CustomNumericLabel2"]);
                        break;
                    case "CustomNumericValue3":
                        headers.Add(column.MappingName, localizations["CustomNumericLabel3"]);
                        break;
                    case "CustomNumericValue4":
                        headers.Add(column.MappingName, localizations["CustomNumericLabel4"]);
                        break;
                    case "CustomTextValue":
                        headers.Add(column.MappingName, localizations["CustomTextLabel"]);
                        break;
                    case "CustomTextValue2":
                        headers.Add(column.MappingName, localizations["CustomTextLabel2"]);
                        break;
                    case "CustomTextValue3":
                        headers.Add(column.MappingName, localizations["CustomTextLabel3"]);
                        break;
                    case "CustomTextValue4":
                        headers.Add(column.MappingName, localizations["CustomTextLabel4"]);
                        break;

                    default:
                        headers.Add(column.MappingName, column.HeaderText);
                        break;
                }
            }
            return headers;
        }

        public static Dictionary<string, ActionHeader> GetColumnHeaderWithValue(
            IEnumerable<SfDataGridGridColumn> visibleColumns,
            Dictionary<string, string> localizations)
        {
            var headers = new Dictionary<string, ActionHeader>();
            foreach (var column in visibleColumns)
            {
                switch (column.HeaderText)
                {
                    case "Refs1":
                        headers.Add(column.MappingName, new ActionHeader { Label = localizations[nameof(Ref1)] , Width = column.Width.ToString().Replace(",",".") } );
                        break;
                    case "Refs2":
                        headers.Add(column.MappingName, new ActionHeader { Label = localizations[nameof(Ref2)], Width = column.Width.ToString().Replace(",", ".") });
                        break;
                    case "Refs3":
                        headers.Add(column.MappingName, new ActionHeader { Label = localizations[nameof(Ref3)], Width = column.Width.ToString().Replace(",", ".") });
                        break;
                    case "Refs4":
                        headers.Add(column.MappingName, new ActionHeader { Label = localizations[nameof(Ref4)], Width = column.Width.ToString().Replace(",", ".") });
                        break;
                    case "Refs5":
                        headers.Add(column.MappingName, new ActionHeader { Label = localizations[nameof(Ref5)], Width = column.Width.ToString().Replace(",", ".") });
                        break;
                    case "Refs6":
                        headers.Add(column.MappingName, new ActionHeader { Label = localizations[nameof(Ref6)], Width = column.Width.ToString().Replace(",", ".") });
                        break;
                    case "Refs7":
                        headers.Add(column.MappingName, new ActionHeader { Label = localizations[nameof(Ref7)], Width = column.Width.ToString().Replace(",", ".") });
                        break;

                    case "CustomNumericValue":
                        headers.Add(column.MappingName, new ActionHeader { Label = localizations["CustomNumericLabel"], Width = column.Width.ToString().Replace(",", ".") });
                        break;
                    case "CustomNumericValue2":
                        headers.Add(column.MappingName, new ActionHeader { Label = localizations["CustomNumericLabel2"], Width = column.Width.ToString().Replace(",", ".") });
                        break;
                    case "CustomNumericValue3":
                        headers.Add(column.MappingName, new ActionHeader { Label = localizations["CustomNumericLabel3"], Width = column.Width.ToString().Replace(",", ".") });
                        break;
                    case "CustomNumericValue4":
                        headers.Add(column.MappingName, new ActionHeader { Label = localizations["CustomNumericLabel4"], Width = column.Width.ToString().Replace(",", ".") });
                        break;
                    case "CustomTextValue":
                        headers.Add(column.MappingName, new ActionHeader { Label = localizations["CustomTextLabel"], Width = column.Width.ToString().Replace(",", ".") });
                        break;
                    case "CustomTextValue2":
                        headers.Add(column.MappingName, new ActionHeader { Label = localizations["CustomTextLabel2"], Width = column.Width.ToString().Replace(",", ".") });
                        break;
                    case "CustomTextValue3":
                        headers.Add(column.MappingName, new ActionHeader { Label = localizations["CustomTextLabel3"], Width = column.Width.ToString().Replace(",", ".") });
                        break;
                    case "CustomTextValue4":
                        headers.Add(column.MappingName, new ActionHeader { Label = localizations["CustomTextLabel4"], Width = column.Width.ToString().Replace(",", ".") });
                        break;

                    default:
                        headers.Add(column.MappingName, new ActionHeader { Label = column.HeaderText, Width = column.Width.ToString().Replace(",", ".") });
                        break;
                }
            }
            return headers;
        }

        public static Dictionary<string, string> GetDetailColumnHeader(
            List<string> visibleColumns,
            Dictionary<string, string> localizations)
        {
            var LocalizedStrings = DependencyResolver.Current.GetService<ILocalizationManager>();
            var headers = new Dictionary<string, string>();
            foreach (var column in visibleColumns)
            {
                switch (column)
                {
                    case nameof(PublishedAction.WBS):
                        headers.Add(column, "Id");
                        break;
                    case nameof(PublishedAction.Label):
                        headers.Add(column, LocalizedStrings.GetString("Grid_LabelHeaderText"));
                        break;
                    case nameof(PublishedAction.Thumbnail):
                        headers.Add(column, LocalizedStrings.GetString("Grid_ThumbnailHeaderText"));
                        break;
                    case nameof(PublishedAction.Duration):
                        headers.Add(column, LocalizedStrings.GetString("Grid_DurationHeaderText"));
                        break;
                    case nameof(PublishedAction.PublishedResource):
                        headers.Add(column, LocalizedStrings.GetString("Grid_PublishedResourceHeaderText"));
                        break;
                    case nameof(PublishedAction.PublishedActionCategory):
                        headers.Add(column, LocalizedStrings.GetString("Grid_PublishedActionCategoryHeaderText"));
                        break;
                    case "Refs1":
                    case "Refs2":
                    case "Refs3":
                    case "Refs4":
                    case "Refs5":
                    case "Refs6":
                    case "Refs7":
                        if (localizations.ContainsKey(column.Replace("s", "")))
                            headers.Add(column, localizations[column.Replace("s", "")]);
                        break;

                    case "CustomNumericValue":
                    case "CustomNumericValue2":
                    case "CustomNumericValue3":
                    case "CustomNumericValue4":
                    case "CustomTextValue":
                    case "CustomTextValue2":
                    case "CustomTextValue3":
                    case "CustomTextValue4":
                        if (localizations.ContainsKey(column.Replace("Value", "Label")))
                            headers.Add(column, localizations[column.Replace("Value", "Label")]);
                        break;

                    default:
                        if (localizations.ContainsKey(column))
                            headers.Add(column, localizations[column]);
                        break;
                }
            }
            return headers;
        }

        public static (List<RefsCollection>, List<CustomLabel>) BuildReferenceAndCustomLabel(
            PublishedAction action,
            Dictionary<string, string> localizations)
        {
            // On construit les détails de la tâche
            var refs = new List<RefsCollection>();
            if (action.PublishedReferentialActions.Any(_ => _.RefNumber == 1))
                refs.Add(new RefsCollection { Field = nameof(PublishedAction.Refs1), Label = localizations[nameof(Ref1)], Values = new TrackableCollection<PublishedReferentialAction>(action.PublishedReferentialActions.Where(_ => _.RefNumber == 1)) });
            if (action.PublishedReferentialActions.Any(_ => _.RefNumber == 2))
                refs.Add(new RefsCollection { Field = nameof(PublishedAction.Refs2), Label = localizations[nameof(Ref2)], Values = new TrackableCollection<PublishedReferentialAction>(action.PublishedReferentialActions.Where(_ => _.RefNumber == 2)) });
            if (action.PublishedReferentialActions.Any(_ => _.RefNumber == 3))
                refs.Add(new RefsCollection { Field = nameof(PublishedAction.Refs3), Label = localizations[nameof(Ref3)], Values = new TrackableCollection<PublishedReferentialAction>(action.PublishedReferentialActions.Where(_ => _.RefNumber == 3)) });
            if (action.PublishedReferentialActions.Any(_ => _.RefNumber == 4))
                refs.Add(new RefsCollection { Field = nameof(PublishedAction.Refs4), Label = localizations[nameof(Ref4)], Values = new TrackableCollection<PublishedReferentialAction>(action.PublishedReferentialActions.Where(_ => _.RefNumber == 4)) });
            if (action.PublishedReferentialActions.Any(_ => _.RefNumber == 5))
                refs.Add(new RefsCollection { Field = nameof(PublishedAction.Refs5), Label = localizations[nameof(Ref5)], Values = new TrackableCollection<PublishedReferentialAction>(action.PublishedReferentialActions.Where(_ => _.RefNumber == 5)) });
            if (action.PublishedReferentialActions.Any(_ => _.RefNumber == 6))
                refs.Add(new RefsCollection { Field = nameof(PublishedAction.Refs6), Label = localizations[nameof(Ref6)], Values = new TrackableCollection<PublishedReferentialAction>(action.PublishedReferentialActions.Where(_ => _.RefNumber == 6)) });
            if (action.PublishedReferentialActions.Any(_ => _.RefNumber == 7))
                refs.Add(new RefsCollection { Field = nameof(PublishedAction.Refs7), Label = localizations[nameof(Ref7)], Values = new TrackableCollection<PublishedReferentialAction>(action.PublishedReferentialActions.Where(_ => _.RefNumber == 7)) });

            var customLabels = new List<CustomLabel>();
            if (!string.IsNullOrEmpty(action.CustomTextValue))
                customLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomTextValue), Label = localizations[nameof(Project.CustomTextLabel)], Value = action.CustomTextValue });
            if (!string.IsNullOrEmpty(action.CustomTextValue2))
                customLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomTextValue2), Label = localizations[nameof(Project.CustomTextLabel2)], Value = action.CustomTextValue2 });
            if (!string.IsNullOrEmpty(action.CustomTextValue3))
                customLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomTextValue3), Label = localizations[nameof(Project.CustomTextLabel3)], Value = action.CustomTextValue3 });
            if (!string.IsNullOrEmpty(action.CustomTextValue4))
                customLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomTextValue4), Label = localizations[nameof(Project.CustomTextLabel4)], Value = action.CustomTextValue4 });
            if (action.CustomNumericValue != null)
                customLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomNumericValue), Label = localizations[nameof(Project.CustomNumericLabel)], Value = action.CustomNumericValue.ToString() });
            if (action.CustomNumericValue2 != null)
                customLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomNumericValue2), Label = localizations[nameof(Project.CustomNumericLabel2)], Value = action.CustomNumericValue2.ToString() });
            if (action.CustomNumericValue3 != null)
                customLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomNumericValue3), Label = localizations[nameof(Project.CustomNumericLabel3)], Value = action.CustomNumericValue3.ToString() });
            if (action.CustomNumericValue4 != null)
                customLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomNumericValue4), Label = localizations[nameof(Project.CustomNumericLabel4)], Value = action.CustomNumericValue4.ToString() });

            return (refs, customLabels);
        }

        public static List<string> GetDetailDispositions(Publication publication, PublishModeEnum publishModeFilter)
        {
            var detailList = new List<string>
            {
                //Add WBS, Label, and Thumbnail as default detail member
                nameof(PublishedAction.WBS),
                nameof(PublishedAction.Label),
                nameof(PublishedAction.Thumbnail)
            };

            string disposition = null;
            switch (publishModeFilter)
            {
                case PublishModeEnum.Formation:
                    disposition = publication.Formation_ActionDisposition;
                    break;
                case PublishModeEnum.Inspection:
                    disposition = publication.Inspection_ActionDisposition;
                    break;
                case PublishModeEnum.Evaluation:
                    disposition = publication.Evaluation_ActionDisposition;
                    break;
            }

            if (string.IsNullOrEmpty(disposition))
                return detailList;

            detailList.AddRange(disposition.Split(','));

            return detailList;
        }

        public static Dictionary<string, string> UpdateLocalizationLabelForDetail(Dictionary<string, string> localization, List<DocumentationReferential> documentationReferentials)
        {
            foreach (var documentationRef in documentationReferentials)
            {
                if (localization.ContainsKey(documentationRef.ReferentialId.ToString()))
                {
                    localization[documentationRef.ReferentialId.ToString()] = documentationRef.Label;
                }
            }
            return localization;
        }
    }
}