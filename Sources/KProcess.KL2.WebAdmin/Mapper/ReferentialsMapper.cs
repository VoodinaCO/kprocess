using KProcess.KL2.WebAdmin.Models.Referentials;
using KProcess.Ksmed.Models;
using Syncfusion.EJ2.Inputs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Hosting;

namespace KProcess.KL2.WebAdmin.Mapper
{
    public static class ReferentialsMapper
    {
        public static string[] ImageExtensions = { ".jpeg", ".jpg", ".png" };

        public static ReferentialsViewModel ToReferentialsViewModel(List<Referential> referentials)
        {
            List<ReferentialViewModel> referentialsViewModel = new List<ReferentialViewModel>();

            foreach (var referential in referentials)
            {
                referentialsViewModel.Add(new ReferentialViewModel {
                    refId = referential.ReferentialId,
                    Label = referential.Label,
                    isResource = referential.ReferentialId == ProcessReferentialIdentifier.Operator || referential.ReferentialId == ProcessReferentialIdentifier.Equipment
                });
            }
            return new ReferentialsViewModel {
                Referentials = referentialsViewModel
            };
        }

        public static (List<RefResourceViewModel>, List<ProcedureViewModel>) ToRefResourceViewModel(int refId,
            List<Operator> operators = null,
            List<Equipment> equipments = null,
            List<ActionCategory> categories = null,
            List<Skill> skills = null,
            List<IMultipleActionReferential> referentials = null,
            List<Procedure> procedures = null,
            List<ActionTypeViewModel> actionTypes = null,
            List<ActionValueViewModel> actionValues = null
            )
        {
            List<RefResourceViewModel> refViewModel = new List<RefResourceViewModel>();
            List<ProcedureViewModel> procedureViewModel = new List<ProcedureViewModel>();
            var identifier = (ProcessReferentialIdentifier)refId;
            //ProcessLabelSort : Sort referential process that it should start with Standard first
            switch (identifier)
            {
                case ProcessReferentialIdentifier.Operator:
                    foreach (var result in operators)
                    {
                        refViewModel.Add(new RefResourceViewModel
                        {
                            itemId = result.Id,
                            Label = result.Label,
                            ProcessId = result.ProcessId,
                            ProcessLabel = result.ProcessId != null ? procedures?.SingleOrDefault(p => p.ProcessId == result.ProcessId)?.Label : "Standard",
                            ProcessLabelSort = result.ProcessId != null ? procedures?.SingleOrDefault(p => p.ProcessId == result.ProcessId)?.Label : string.Empty,
                            Color = result.Color != null ? $"#{result.Color.Substring(3)}" : string.Empty,
                            PaceRating = result.PaceRating,
                            Hash = result.CloudFile?.Hash,
                            Extension = result.CloudFile?.Extension,
                            Description = result.Description,
                            FileType = GetFileType(result.CloudFile?.Extension),
                            CloudFileSize = result.CloudFile == null ? (double?)null : GetFileSize(result.CloudFile.FileName)
                        }); ;
                    }
                    foreach (var result in procedures.OrderBy(p => p.Label))
                    {
                        procedureViewModel.Add(new ProcedureViewModel {
                            Id = result.ProcessId,
                            Label = result.Label
                        });
                    }
                    break;
                case ProcessReferentialIdentifier.Equipment:
                    foreach (var result in equipments)
                    {
                        refViewModel.Add(new RefResourceViewModel
                        {
                            itemId = result.Id,
                            Label = result.Label,
                            ProcessId = result.ProcessId,
                            ProcessLabel = result.ProcessId != null ? procedures?.SingleOrDefault(p => p.ProcessId == result.ProcessId)?.Label : "Standard",
                            ProcessLabelSort = result.ProcessId != null ? procedures?.SingleOrDefault(p => p.ProcessId == result.ProcessId)?.Label : string.Empty,
                            Color = result.Color != null ? $"#{result.Color.Substring(3)}" : string.Empty,
                            PaceRating = result.PaceRating,
                            Hash = result.CloudFile?.Hash,
                            Extension = result.CloudFile?.Extension,
                            Description = result.Description,
                            FileType = GetFileType(result.CloudFile?.Extension),
                            CloudFileSize = result.CloudFile == null ? (double?)null : GetFileSize(result.CloudFile.FileName)
                        });
                    }
                    foreach (var result in procedures.OrderBy(p => p.Label))
                    {
                        procedureViewModel.Add(new ProcedureViewModel
                        {
                            Id = result.ProcessId,
                            Label = result.Label
                        });
                    }
                    break;
                case ProcessReferentialIdentifier.Category:
                    foreach (var result in categories)
                    {
                        refViewModel.Add(new RefResourceViewModel {
                            itemId = result.ActionCategoryId,
                            Label = result.Label,
                            ProcessId = result.ProcessId,
                            ProcessLabel = result.ProcessId != null ? procedures?.SingleOrDefault(p => p.ProcessId == result.ProcessId)?.Label : "Standard",
                            ProcessLabelSort = result.ProcessId != null ? procedures?.SingleOrDefault(p => p.ProcessId == result.ProcessId)?.Label : string.Empty,
                            Color = result.Color != null ? $"#{result.Color.Substring(3)}" : string.Empty,
                            Hash = result.CloudFile?.Hash,
                            Extension = result.CloudFile?.Extension,
                            Description = result.Description,
                            FileType = GetFileType(result.CloudFile?.Extension),
                            CloudFileSize = result.CloudFile == null ? (double?)null : GetFileSize(result.CloudFile.FileName),
                            ActionTypeCode = result.ActionTypeCode,
                            ActionTypeLabel = result.ActionTypeCode != null ? actionTypes.FirstOrDefault(a => a.Code == result.ActionTypeCode).Label : "",
                            ActionValueCode = result.ActionValueCode,
                            ActionValueLabel = actionValues.FirstOrDefault(a => a.Code == result.ActionValueCode).Label
                        });
                    }
                    foreach (var result in procedures.OrderBy(p => p.Label))
                    {
                        procedureViewModel.Add(new ProcedureViewModel
                        {
                            Id = result.ProcessId,
                            Label = result.Label
                        });
                    }
                    break;
                case ProcessReferentialIdentifier.Skill:
                    foreach (var result in skills)
                    {
                        refViewModel.Add(new RefResourceViewModel
                        {
                            itemId = result.Id,
                            Label = result.Label,
                            Color = result.Color != null ? $"#{result.Color.Substring(3)}" : string.Empty,
                            Hash = result.CloudFile?.Hash,
                            Extension = result.CloudFile?.Extension,
                            Description = result.Description,
                            FileType = GetFileType(result.CloudFile?.Extension),
                            CloudFileSize = result.CloudFile == null ? (double?)null : GetFileSize(result.CloudFile.FileName)
                        });
                    }
                    if (procedures != null)
                    {
                        foreach (var result in procedures.OrderBy(p => p.Label))
                        {
                            procedureViewModel.Add(new ProcedureViewModel
                            {
                                Id = result.ProcessId,
                                Label = result.Label
                            });
                        }
                    }
                    break;
                case ProcessReferentialIdentifier.Ref1:
                case ProcessReferentialIdentifier.Ref2:
                case ProcessReferentialIdentifier.Ref3:
                case ProcessReferentialIdentifier.Ref4:
                case ProcessReferentialIdentifier.Ref5:
                case ProcessReferentialIdentifier.Ref6:
                case ProcessReferentialIdentifier.Ref7:
                    foreach (var result in referentials.OfType<IActionReferentialProcess>())
                    {
                        refViewModel.Add(new RefResourceViewModel
                        {
                            itemId = result.Id,
                            Label = result.Label,
                            ProcessId = result.ProcessId,
                            ProcessLabel = result.ProcessId != null ? procedures?.SingleOrDefault(p => p.ProcessId == result.ProcessId)?.Label : "Standard",
                            ProcessLabelSort = result.ProcessId != null ? procedures?.SingleOrDefault(p => p.ProcessId == result.ProcessId)?.Label : string.Empty,
                            Color = result.Color != null ? $"#{result.Color.Substring(3)}" : string.Empty,
                            Hash = result.CloudFile?.Hash,
                            Extension = result.CloudFile?.Extension,
                            Description = result.Description,
                            FileType = GetFileType(result.CloudFile?.Extension),
                            CloudFileSize = result.CloudFile == null ? (double?)null : GetFileSize(result.CloudFile.FileName)
                        });
                    }
                    foreach (var result in procedures.OrderBy(p => p.Label))
                    {
                        procedureViewModel.Add(new ProcedureViewModel
                        {
                            Id = result.ProcessId,
                            Label = result.Label
                        });
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return (refViewModel,procedureViewModel);
        }

        public static string GetAppFilePath(int intRefIdentifier, int itemId, string extension) =>
            $"Files/{intRefIdentifier}.{itemId}.{extension}";

        public static bool IsImage(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return false;
            return ImageExtensions.Contains(extension.ToLowerInvariant());
        }

        public static string GetServerFilePath(int intRefIdentifier, int itemId, string extension) =>
            Path.Combine(HostingEnvironment.MapPath("~/Files"), $"{intRefIdentifier}.{itemId}.{extension}");

        public static string GetUri(string filename) =>
            string.IsNullOrEmpty(filename) ? null : $"{WebConfigurationManager.AppSettings["FileServerUri"]}/GetFile/{filename}";

        public static long GetFileSize(string filename)
        {
            WebRequest webRequest = WebRequest.Create(WebConfigurationManager.AppSettings["FileServerUri"] + "/GetFileSize/" + filename);
            WebResponse webResp = webRequest.GetResponse();
            using (Stream stream = webResp.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                string responseString = reader.ReadToEnd().Replace("\"", "");
                return long.Parse(responseString);
            }
        }

        public static string GetFileType(string extension) =>
            string.IsNullOrEmpty(extension) ? null : (IsImage(extension) ? "image" : "file");
    }
}