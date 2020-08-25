using KProcess.KL2.Languages;
using KProcess.KL2.WebAdmin.Models.Action;
using KProcess.KL2.WebAdmin.Models.Documentation;
using KProcess.KL2.WebAdmin.Utils;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KProcess.KL2.WebAdmin.Mapper
{
    public static class DocumentationMapper
    {
        static readonly List<string> ImageExtensions = new List<string> { ".png", ".jpg" };

        public static DocumentationActionDraft MapToDocumentationActionDraft(this GenericActionViewModel model, PublishModeEnum publishMode, Dictionary<int, string> wbsActions)
        {
            var action = new DocumentationActionDraft
            {
                Label = model.Label,
                Duration = model.Duration,
                SkillId = model.SkillId,
                IsKeyTask = model.IsKeyTask,
                ThumbnailHash = model.ImageHash,
                CustomTextValue = model.CustomTextValues != null ? model.CustomTextValues.GetOrDefault(ProcessReferentialIdentifier.CustomTextLabel) : string.Empty,
                CustomTextValue2 = model.CustomTextValues != null ? model.CustomTextValues.GetOrDefault(ProcessReferentialIdentifier.CustomTextLabel2) : string.Empty,
                CustomTextValue3 = model.CustomTextValues != null ? model.CustomTextValues.GetOrDefault(ProcessReferentialIdentifier.CustomTextLabel3) : string.Empty,
                CustomTextValue4 = model.CustomTextValues != null ? model.CustomTextValues.GetOrDefault(ProcessReferentialIdentifier.CustomTextLabel4) : string.Empty,
                CustomNumericValue = model.CustomNumericValues != null ? model.CustomNumericValues.GetOrDefault(ProcessReferentialIdentifier.CustomNumericLabel) : null,
                CustomNumericValue2 = model.CustomNumericValues != null ? model.CustomNumericValues.GetOrDefault(ProcessReferentialIdentifier.CustomNumericLabel2) : null,
                CustomNumericValue3 = model.CustomNumericValues != null ? model.CustomNumericValues.GetOrDefault(ProcessReferentialIdentifier.CustomNumericLabel3) : null,
                CustomNumericValue4 = model.CustomNumericValues != null ? model.CustomNumericValues.GetOrDefault(ProcessReferentialIdentifier.CustomNumericLabel4) : null
            };

            if (model.ReferentialFieldValues == null)
                return action;

            // Build reference
            foreach(var reference in model.ReferentialFieldValues)
            {
                foreach (var referenceValue in reference.Values)
                {
                    action.ReferentialDocumentations.Add(new ReferentialDocumentationActionDraft
                    {
                        Quantity = referenceValue.Quantity,
                        ReferentialId = referenceValue.ReferentialId,
                        RefNumber = (int)reference.ReferentialFieldId - 3
                    });
                }
            }
            return action;
        }


        /// <summary>
        /// Retrieve all the headers that needs to be displayed in the grid
        /// </summary>
        /// <param name="project"></param>
        /// <param name="serviceReferentials"></param>
        /// <param name="includeOnlyReferentialsAndCustom"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, string>> GetHeaders(
            int processId,
            IReferentialsService referentialsService,
            bool includeOnlyReferentialsAndCustom = false)
        {
            var LocalizedStrings = DependencyResolver.Current.GetService<ILocalizationManager>();

            var documentationReferentials = await referentialsService.GetDocumentationReferentials(processId);

            var headers = !includeOnlyReferentialsAndCustom ? new Dictionary<string, string>
            {
                [nameof(PublishedAction.WBS)] = "Id",
                [nameof(PublishedAction.Label)] = LocalizedStrings.GetString("Grid_LabelHeaderText"),
                [nameof(PublishedAction.Thumbnail)] = LocalizedStrings.GetString("Grid_ThumbnailHeaderText"),
                [nameof(PublishedAction.Duration)] = LocalizedStrings.GetString("Grid_DurationHeaderText"),
                [nameof(PublishedAction.PublishedResource)] = LocalizedStrings.GetString("Grid_PublishedResourceHeaderText")
            } : new Dictionary<string, string>();

            foreach (var docRef in documentationReferentials.Where(_ => _.IsEnabled))
            {
                switch (docRef.ReferentialId)
                {
                    case ProcessReferentialIdentifier.Category:
                        headers.Add(nameof(PublishedAction.PublishedActionCategory),
                            docRef.Label);
                        break;
                    case ProcessReferentialIdentifier.Skill:
                        headers.Add(docRef.ReferentialId.ToString(),
                            docRef.Label);
                        break;
                    case ProcessReferentialIdentifier.Ref1:
                    case ProcessReferentialIdentifier.Ref2:
                    case ProcessReferentialIdentifier.Ref3:
                    case ProcessReferentialIdentifier.Ref4:
                    case ProcessReferentialIdentifier.Ref5:
                    case ProcessReferentialIdentifier.Ref6:
                    case ProcessReferentialIdentifier.Ref7:
                        headers.Add(docRef.ReferentialId.ToString().Insert(3, "s"),
                            docRef.Label);
                        break;
                    case ProcessReferentialIdentifier.CustomTextLabel:
                        headers.Add(nameof(PublishedAction.CustomTextValue), docRef.Label);
                        break;
                    case ProcessReferentialIdentifier.CustomTextLabel2:
                        headers.Add(nameof(PublishedAction.CustomTextValue2), docRef.Label);
                        break;
                    case ProcessReferentialIdentifier.CustomTextLabel3:
                        headers.Add(nameof(PublishedAction.CustomTextValue3), docRef.Label);
                        break;
                    case ProcessReferentialIdentifier.CustomTextLabel4:
                        headers.Add(nameof(PublishedAction.CustomTextValue4), docRef.Label);
                        break;
                    case ProcessReferentialIdentifier.CustomNumericLabel:
                        headers.Add(nameof(PublishedAction.CustomNumericValue), docRef.Label);
                        break;
                    case ProcessReferentialIdentifier.CustomNumericLabel2:
                        headers.Add(nameof(PublishedAction.CustomNumericValue2), docRef.Label);
                        break;
                    case ProcessReferentialIdentifier.CustomNumericLabel3:
                        headers.Add(nameof(PublishedAction.CustomNumericValue3), docRef.Label);
                        break;
                    case ProcessReferentialIdentifier.CustomNumericLabel4:
                        headers.Add(nameof(PublishedAction.CustomNumericValue4), docRef.Label);
                        break;
                }
            }

            return headers;
        }

        public static async Task<IList<ReferentialFieldElement>> GetReferentialValues(ProcessReferentialIdentifier identifier, bool hasQuantity, int processId)
        {
            var LocalizedStrings = DependencyResolver.Current.GetService<ILocalizationManager>();
            var referentialService = DependencyResolver.Current.GetService<IReferentialsService>();
            IEnumerable<IActionReferential> refs = new List<IActionReferential>();
            switch (identifier)
            {
                case ProcessReferentialIdentifier.Operator:
                case ProcessReferentialIdentifier.Equipment:
                case ProcessReferentialIdentifier.Category:
                case ProcessReferentialIdentifier.Skill:
                case ProcessReferentialIdentifier.Ref1:
                case ProcessReferentialIdentifier.Ref2:
                case ProcessReferentialIdentifier.Ref3:
                case ProcessReferentialIdentifier.Ref4:
                case ProcessReferentialIdentifier.Ref5:
                case ProcessReferentialIdentifier.Ref6:
                case ProcessReferentialIdentifier.Ref7:
                    var (Referentials, _) = await referentialService.GetAllReferentials(identifier, processId);
                    refs = Referentials;
                    break;
                default:
                    break;
            }
            if (identifier == ProcessReferentialIdentifier.Operator || identifier == ProcessReferentialIdentifier.Equipment)
            {
                return refs.Select(u => new ReferentialFieldElement
                {
                    Id = u.Id,
                    Label = u.Label,
                    HasQuantity = hasQuantity,
                    CategoryAsLabel = u.ProcessReferentialId == ProcessReferentialIdentifier.Operator ? LocalizedStrings.GetString( "Common_Referential_Operator") : LocalizedStrings.GetString( "Common_Referential_Equipment"),
                    Category = u.ProcessReferentialId == ProcessReferentialIdentifier.Operator ? 0 : 1,
                    CloudFile = u.CloudFile,
                    Description = u.Description
                }).OrderBy(u => u.Category).ToList();
            }
            else if (identifier == ProcessReferentialIdentifier.Skill)
            {
                return refs.Select(u => new ReferentialFieldElement
                {
                    Id = u.Id,
                    Label = u.Label,
                    HasQuantity = hasQuantity,
                    CategoryAsLabel = LocalizedStrings.GetString( "VM_ReferentialsGroupSortDescription_Standard"),
                    Category = 0,
                    CloudFile = u.CloudFile,
                    Description = u.Description
                }).OrderBy(u => u.Category).ToList();
            }
            else
            {
                return refs.Select(u => new ReferentialFieldElement
                {
                    Id = u.Id,
                    Label = u.Label,
                    HasQuantity = hasQuantity,
                    CategoryAsLabel = ((IActionReferentialProcess)u).ProcessId == null ? LocalizedStrings.GetString( "VM_ReferentialsGroupSortDescription_Standard") : ((IActionReferentialProcess)u).Process.Label,
                    Category = ((IActionReferentialProcess)u).ProcessId == null ? 0 : 1,
                    CloudFile = u.CloudFile,
                    Description = u.Description
                }).OrderBy(u => u.Category).ToList();
            }
        }



        /// <summary>
        /// Retrieve all the headers that needs to be displayed in the grid
        /// </summary>
        /// <param name="serviceReferentials"></param>
        /// <param name="documentationReferentials"></param>
        /// <returns></returns>
        public static Task<List<FormatActionsElementViewModel>> GetRefsAsync(
            IEnumerable<Referential> serviceReferentials,
            List<DocumentationReferential> documentationReferentials,
            string actionDisposition)
        {
            var LocalizedStrings = DependencyResolver.Current.GetService<ILocalizationManager>();
            return Task.Run(() =>
            {
                var actionDispositionFields = actionDisposition?.Split(',') ?? new string[0];
                var model = new Dictionary<string, FormatActionsElementValue>();

                var enabledReferentials = documentationReferentials.Where(r => r.IsEnabled == true).Select(r => r.ReferentialId).ToList();
                var enabledReferentialsString = enabledReferentials.Select(r => r.ToString()).ToList();
                serviceReferentials = serviceReferentials.Where(sr => enabledReferentials.Any(er => er.Equals(sr.ReferentialId)));

                //Duration
                model.Add(nameof(PublishedAction.Duration), new FormatActionsElementValue { Label = LocalizedStrings.GetString("Grid_DurationHeaderText"), Category = ReferentialCategory.Other });

                //PublishedResource
                model.Add(nameof(PublishedAction.PublishedResource), new FormatActionsElementValue { Label = LocalizedStrings.GetString("Grid_PublishedResourceHeaderText"), Category = ReferentialCategory.Other });


                //used referentials
                foreach (var refe in serviceReferentials)
                {
                    var referential = serviceReferentials.FirstOrDefault(r => r.ReferentialId == refe.ReferentialId);
                    var documentationReferential = documentationReferentials.FirstOrDefault(r => r.ReferentialId == referential.ReferentialId);
                    if (documentationReferential == null)
                        continue;

                    var label = documentationReferential.Label;
                    var name = referential.ReferentialId.ToString();

                    switch (refe.ReferentialId)
                    {
                        case ProcessReferentialIdentifier.Ref1:
                        case ProcessReferentialIdentifier.Ref2:
                        case ProcessReferentialIdentifier.Ref3:
                        case ProcessReferentialIdentifier.Ref4:
                        case ProcessReferentialIdentifier.Ref5:
                        case ProcessReferentialIdentifier.Ref6:
                        case ProcessReferentialIdentifier.Ref7:
                            name = refe.ReferentialId.ToString().Insert(3, "s");
                            model.Add(name, new FormatActionsElementValue { Label = label, Category = ProcessReferentialIdentifierExt.GetReferentialCategory(refe.ReferentialId) });
                            break;
                        default:
                            model.Add(name, new FormatActionsElementValue { Label = label, Category = ProcessReferentialIdentifierExt.GetReferentialCategory(refe.ReferentialId) });
                            break;
                    }
                }

                //Custom text/numeric
                if (enabledReferentialsString.Contains(nameof(ProcessReferentialIdentifier.CustomTextLabel)))
                    model.Add(nameof(PublishedAction.CustomTextValue), new FormatActionsElementValue { Label = documentationReferentials.FirstOrDefault(r => r.ReferentialId.ToString() == nameof(ProcessReferentialIdentifier.CustomTextLabel)).Label, Category = ReferentialCategory.CustomTextField });
                if (enabledReferentialsString.Contains(nameof(ProcessReferentialIdentifier.CustomTextLabel2)))
                    model.Add(nameof(PublishedAction.CustomTextValue2), new FormatActionsElementValue { Label = documentationReferentials.FirstOrDefault(r => r.ReferentialId.ToString() == nameof(ProcessReferentialIdentifier.CustomTextLabel2)).Label, Category = ReferentialCategory.CustomTextField });
                if (enabledReferentialsString.Contains(nameof(ProcessReferentialIdentifier.CustomTextLabel3)))
                    model.Add(nameof(PublishedAction.CustomTextValue3), new FormatActionsElementValue { Label = documentationReferentials.FirstOrDefault(r => r.ReferentialId.ToString() == nameof(ProcessReferentialIdentifier.CustomTextLabel3)).Label, Category = ReferentialCategory.CustomTextField });
                if (enabledReferentialsString.Contains(nameof(ProcessReferentialIdentifier.CustomTextLabel4)))
                    model.Add(nameof(PublishedAction.CustomTextValue4), new FormatActionsElementValue { Label = documentationReferentials.FirstOrDefault(r => r.ReferentialId.ToString() == nameof(ProcessReferentialIdentifier.CustomTextLabel4)).Label, Category = ReferentialCategory.CustomTextField });
                if (enabledReferentialsString.Contains(nameof(ProcessReferentialIdentifier.CustomNumericLabel)))
                    model.Add(nameof(PublishedAction.CustomNumericValue), new FormatActionsElementValue { Label = documentationReferentials.FirstOrDefault(r => r.ReferentialId.ToString() == nameof(ProcessReferentialIdentifier.CustomNumericLabel)).Label, Category = ReferentialCategory.CustomNumericField });
                if (enabledReferentialsString.Contains(nameof(ProcessReferentialIdentifier.CustomNumericLabel2)))
                    model.Add(nameof(PublishedAction.CustomNumericValue2), new FormatActionsElementValue { Label = documentationReferentials.FirstOrDefault(r => r.ReferentialId.ToString() == nameof(ProcessReferentialIdentifier.CustomNumericLabel2)).Label, Category = ReferentialCategory.CustomNumericField });
                if (enabledReferentialsString.Contains(nameof(ProcessReferentialIdentifier.CustomNumericLabel3)))
                    model.Add(nameof(PublishedAction.CustomNumericValue3), new FormatActionsElementValue { Label = documentationReferentials.FirstOrDefault(r => r.ReferentialId.ToString() == nameof(ProcessReferentialIdentifier.CustomNumericLabel3)).Label, Category = ReferentialCategory.CustomNumericField });
                if (enabledReferentialsString.Contains(nameof(ProcessReferentialIdentifier.CustomNumericLabel4)))
                    model.Add(nameof(PublishedAction.CustomNumericValue4), new FormatActionsElementValue { Label = documentationReferentials.FirstOrDefault(r => r.ReferentialId.ToString() == nameof(ProcessReferentialIdentifier.CustomNumericLabel4)).Label, Category = ReferentialCategory.CustomNumericField });

                var referentials = model.Select(u => new FormatActionsElementViewModel
                {
                    Text = u.Value.Label,
                    MappingName = u.Key,
                    Category = u.Value.Category,
                    CategoryKey = (int)u.Value.Category,
                    IsChecked = actionDispositionFields.Contains(u.Key)
                }).ToList();
                foreach (var referential in referentials)
                {
                    var translated = "";
                    switch (referential.Category)
                    {
                        case ReferentialCategory.Referential:
                            translated = LocalizedStrings.GetString("View_Common_Referential_Label");
                            break;
                        case ReferentialCategory.CustomTextField:
                            translated = LocalizedStrings.GetString("View_Common_CustomFieldText_Label");
                            break;
                        case ReferentialCategory.CustomNumericField:
                            translated = LocalizedStrings.GetString("View_Common_CustomFieldNumeric_Label");
                            break;
                        default:
                            translated = LocalizedStrings.GetString("View_Common_Other_Label");
                            break;
                    }
                    referentials.FirstOrDefault(r => r.MappingName == referential.MappingName).TranslatedCategory = translated;
                }
                return referentials;
            });
        }
            

        public static ActionColumnViewModel GetActionValues(
            string header, 
            KAction action, 
            IEnumerable<ProjectReferential> usedReferentials, 
            Dictionary<int, string> wbsValues = null)
        {
            var model = new ActionColumnViewModel { Values = new List<ActionValueViewModel>() };

            switch (header)
            {
                case nameof(PublishedAction.WBS):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = wbsValues?.GetOrDefault(action.ActionId) ?? action.WBS
                    });
                    break;
                case nameof(PublishedAction.Label):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = action.Label ?? string.Empty
                    });
                    break;
                case nameof(PublishedAction.Thumbnail):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Image",
                        Value = action.Thumbnail?.Hash,
                        FileHash = action.Thumbnail?.Hash,
                        FileExt = action.Thumbnail?.Extension
                    });
                    break;
                case nameof(PublishedAction.Duration):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = TicksUtil.TicksToString(action.Duration, action.Scenario.Project.TimeScale)
                    });
                    break;
                case nameof(PublishedAction.PublishedResource):
                    model.Values.Add(BuildFile(action.Operator?.CloudFile ?? action.Equipment?.CloudFile,
                        action.Operator?.Label ?? action.Equipment?.Label, 1));
                    break;
                case nameof(PublishedAction.Refs1):
                    model.Values = BuildReferentialFile(action.Refs(ProcessReferentialIdentifier.Ref1), usedReferentials);
                    break;
                case nameof(PublishedAction.Refs2):
                    model.Values = BuildReferentialFile(action.Refs(ProcessReferentialIdentifier.Ref2), usedReferentials);
                    break;
                case nameof(PublishedAction.Refs3):
                    model.Values = BuildReferentialFile(action.Refs(ProcessReferentialIdentifier.Ref3), usedReferentials);
                    break;
                case nameof(PublishedAction.Refs4):
                    model.Values = BuildReferentialFile(action.Refs(ProcessReferentialIdentifier.Ref4), usedReferentials);
                    break;
                case nameof(PublishedAction.Refs5):
                    model.Values = BuildReferentialFile(action.Refs(ProcessReferentialIdentifier.Ref5), usedReferentials);
                    break;
                case nameof(PublishedAction.Refs6):
                    model.Values = BuildReferentialFile(action.Refs(ProcessReferentialIdentifier.Ref6), usedReferentials);
                    break;
                case nameof(PublishedAction.Refs7):
                    model.Values = BuildReferentialFile(action.Refs(ProcessReferentialIdentifier.Ref7), usedReferentials);
                    break;
                default:
                    var attribute = ReflectionHelper.GetPropertyValue(action, header);
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = attribute != null ? attribute.ToString() : ""
                    });
                    break;
            }
            return model;
        }

        public static ActionColumnViewModel GetActionValues(string header,
            GenericActionViewModel action,
            Dictionary<int, string> wbsValues = null)
        {
            var model = new ActionColumnViewModel { Values = new List<ActionValueViewModel>() };

            switch (header)
            {
                case nameof(PublishedAction.WBS):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = wbsValues?.GetOrDefault(action.ActionId) ?? action.WBS
                    });
                    break;
                case nameof(PublishedAction.Label):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = action.Label ?? string.Empty
                    });
                    break;
                case nameof(PublishedAction.Thumbnail):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Image",
                        Value = action.ImageHash,
                        FileHash = action.ImageHash,
                        FileExt = action.Extension
                    });
                    break;
                case nameof(PublishedAction.Duration):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = TicksUtil.TicksToString(action.Duration, action.TimeScale)
                    });
                    break;
                case nameof(PublishedAction.Refs1):
                    model.Values = BuildMultiValueRefs(action, ProcessReferentialIdentifier.Ref1);
                    break;
                case nameof(PublishedAction.Refs2):
                    model.Values = BuildMultiValueRefs(action, ProcessReferentialIdentifier.Ref2);
                    break;
                case nameof(PublishedAction.Refs3):
                    model.Values = BuildMultiValueRefs(action, ProcessReferentialIdentifier.Ref3);
                    break;
                case nameof(PublishedAction.Refs4):
                    model.Values = BuildMultiValueRefs(action, ProcessReferentialIdentifier.Ref4);
                    break;
                case nameof(PublishedAction.Refs5):
                    model.Values = BuildMultiValueRefs(action, ProcessReferentialIdentifier.Ref5);
                    break;
                case nameof(PublishedAction.Refs6):
                    model.Values = BuildMultiValueRefs(action, ProcessReferentialIdentifier.Ref6);
                    break;
                case nameof(PublishedAction.Refs7):
                    model.Values = BuildMultiValueRefs(action, ProcessReferentialIdentifier.Ref7);
                    break;
                case nameof(PublishedAction.CustomTextValue):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = action.CustomTextValues != null ? action.CustomTextValues
                            .Where(t => t.Key == ProcessReferentialIdentifier.CustomTextLabel)
                            .Select(t => t.Value)
                            .DefaultIfEmpty(string.Empty)
                            .FirstOrDefault() : string.Empty
                    });
                    break;
                case nameof(PublishedAction.CustomTextValue2):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = action.CustomTextValues != null ? action.CustomTextValues
                            .Where(t => t.Key == ProcessReferentialIdentifier.CustomTextLabel2)
                            .Select(t => t.Value)
                            .DefaultIfEmpty(string.Empty)
                            .FirstOrDefault() : string.Empty
                    });
                    break;
                case nameof(PublishedAction.CustomTextValue3):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = action.CustomTextValues != null ? action.CustomTextValues
                            .Where(t => t.Key == ProcessReferentialIdentifier.CustomTextLabel3)
                            .Select(t => t.Value)
                            .DefaultIfEmpty(string.Empty)
                            .FirstOrDefault() : string.Empty
                    });
                    break;
                case nameof(PublishedAction.CustomTextValue4):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = action.CustomTextValues != null ? action.CustomTextValues
                            .Where(t => t.Key == ProcessReferentialIdentifier.CustomTextLabel4)
                            .Select(t => t.Value)
                            .DefaultIfEmpty(string.Empty)
                            .FirstOrDefault() : string.Empty
                    });
                    break;
                case nameof(PublishedAction.CustomNumericValue):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = action.CustomNumericFields != null ? action.CustomNumericValues
                            .Where(t => t.Key == ProcessReferentialIdentifier.CustomNumericLabel)
                            .Select(t => $"{t.Value}")
                            .DefaultIfEmpty(string.Empty)
                            .FirstOrDefault() : string.Empty
                    });
                    break;
                case nameof(PublishedAction.CustomNumericValue2):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = action.CustomNumericFields != null ? action.CustomNumericValues
                            .Where(t => t.Key == ProcessReferentialIdentifier.CustomNumericLabel2)
                            .Select(t => $"{t.Value}")
                            .DefaultIfEmpty(string.Empty)
                            .FirstOrDefault() : string.Empty
                    });
                    break;
                case nameof(PublishedAction.CustomNumericValue3):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = action.CustomNumericFields != null ? action.CustomNumericValues
                            .Where(t => t.Key == ProcessReferentialIdentifier.CustomNumericLabel3)
                            .Select(t => $"{t.Value}")
                            .DefaultIfEmpty(string.Empty)
                            .FirstOrDefault() : string.Empty
                    });
                    break;
                case nameof(PublishedAction.CustomNumericValue4):
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = action.CustomNumericFields != null ? action.CustomNumericValues
                            .Where(t => t.Key == ProcessReferentialIdentifier.CustomNumericLabel4)
                            .Select(t => $"{t.Value}")
                            .DefaultIfEmpty(string.Empty)
                            .FirstOrDefault() : string.Empty
                    });
                    break;
                default:
                    var attribute = ReflectionHelper.GetPropertyValue(action, header);
                    model.Values.Add(new ActionValueViewModel
                    {
                        Type = "Text",
                        Value = attribute != null ? attribute.ToString() : string.Empty
                    });
                    break;
            }
            return model;
        }

        public static List<ActionValueViewModel> BuildMultiValueRefs(GenericActionViewModel action, ProcessReferentialIdentifier refId)
        {
            var model = new List<ActionValueViewModel>();
            var actionRefs = action.ReferentialFieldValues.FirstOrDefault(r => r.ReferentialFieldId == refId);
            var refField = action.ReferentialsFields.FirstOrDefault(r => r.ReferentialFieldId == refId);
            
            if (actionRefs != null && actionRefs.Values.Count != 0 && refField != null)
            {
                var refFieldElements = refField.ReferentialsFieldElements;
                var refValues = actionRefs.Values;
                
                foreach (var refValue in refValues)
                {
                    var refe = refFieldElements.FirstOrDefault(r => r.Id == refValue.ReferentialId);
                    if (refe != null)
                    {
                        var fileModel = BuildFile(null, refe.Label, refValue.Quantity);
                        model.Add(fileModel);
                    }
                }
            }
            return model;
        }


        public static List<ActionValueViewModel> BuildReferentialFile(IEnumerable<IReferentialActionLink> refs, IEnumerable<ProjectReferential> usedReferentials)
        {
            var values = new List<ActionValueViewModel>();
            if (refs?.Any() != true)
                return values;
            var referentialSetting = usedReferentials.FirstOrDefault(r => r.ReferentialId == refs.FirstOrDefault().Referential.ProcessReferentialId);
            if (referentialSetting != null)
            {
                foreach (var referential in refs)
                {
                    var fileModel = BuildFile(
                        referential.Referential.CloudFile,
                        referential.Referential.Label,
                        referentialSetting.HasQuantity == true ? referential.Quantity : (int?)null);
                    values.Add(fileModel);
                }
            }            
            return values;
        }

        /// <summary>
        /// Build action view model from cloud file
        /// </summary>
        /// <param name="label"></param>
        /// <param name="cloudFile"></param>
        /// <returns></returns>
        public static ActionValueViewModel BuildFile(CloudFile cloudFile, string label, int? quantity)
        {
            // Referential is only text
            if (cloudFile == null)
                return new ActionValueViewModel
                {
                    Type = "Text",
                    Value = label ?? string.Empty,
                    Quantity = quantity
                };
            // Published file is an image
            if (ImageExtensions.Contains(cloudFile.Extension))
                return new ActionValueViewModel
                {
                    Type = "Image",
                    Description = label,
                    Value = cloudFile.Hash,
                    FileHash = cloudFile.Hash,
                    FileExt = cloudFile.Extension,
                    Quantity = quantity
                };
            // Other type of file
            return new ActionValueViewModel
            {
                Type = "File",
                Description = label,
                Value = label ?? string.Empty,
                FileHash = cloudFile.Hash,
                FileExt = cloudFile.Extension
            };
        }

        public static DocumentationHistory PublicationHistoryToDocumentation(PublicationHistory history)
        {
            var LocalizedStrings = DependencyResolver.Current.GetService<ILocalizationManager>();
            var result = new DocumentationHistory
            {
                ProcessId = history.ProcessId,
                PublicationHistoryId = history.PublicationHistoryId,
                ProcessLabel = history.PublishedProcess.Label,
                Description = GetHistoryDescription(history),
                TrainingVersion = history.TrainingPublicationVersion,
                EvaluationVersion = history.EvaluationPublicationVersion,
                InspectionVersion = history.InspectionPublicationVersion,
                State = history.State,
                StateAsString = history.State.ToString().ToLower(),
                StateAsLabel = LocalizedStrings.GetString($"View_DocumentationHistory_State_{history.State.ToString()}"),
                Timestamp = history.Timestamp,
                ErrorMessage = history.ErrorMessage,
                Publisher = history.Publisher.FullName
            };
            return result;
        }

        /// <summary>
        /// Build description for this publication history
        /// </summary>
        /// <param name="history"></param>
        /// <returns></returns>
        public static string GetHistoryDescription(PublicationHistory history)
        {
            var versions = new List<string>();

            if (history.TrainingPublication != null || history.TrainingPublicationVersion != null)
                versions.Add(GetDocumentationDescription(history.TrainingPublication, PublishModeEnum.Formation, history.TrainingPublicationVersion));
            if (history.EvaluationPublication != null || history.EvaluationPublicationVersion != null)
                versions.Add(GetDocumentationDescription(history.EvaluationPublication, PublishModeEnum.Evaluation, history.EvaluationPublicationVersion));
            if (history.InspectionPublication != null || history.InspectionPublicationVersion != null)
                versions.Add(GetDocumentationDescription(history.InspectionPublication, PublishModeEnum.Inspection, history.InspectionPublicationVersion));
            return string.Join(" | ", versions);
        }

        /// <summary>
        /// Build publication description. Version is displayed only if publiation is completed
        /// </summary>
        /// <param name="history"></param>
        /// <param name="publication"></param>
        /// <returns></returns>
        public static string GetDocumentationDescription(Publication publication, PublishModeEnum publishMode, string historyVersion)
        {
            return publication != null 
                ? $"{publication.PublishMode.ToString().ToUpper()} {publication.Version}" 
                : $"{publishMode.ToString().ToUpper()} {historyVersion}";
        }
    }
}