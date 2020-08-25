using KProcess.KL2.Languages;
using KProcess.KL2.WebAdmin.Authentication;
using KProcess.KL2.WebAdmin.Identity;
using KProcess.KL2.WebAdmin.Mapper;
using KProcess.KL2.WebAdmin.Models.Action;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KProcess.KL2.WebAdmin.Controllers
{
    [CustomAuthorize(KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Technician, KnownRoles.Operator, KnownRoles.Documentalist)]
    [SettingUserContextFilter]
    public class ActionController : LocalizedController
    {
        readonly IPrepareService _prepareService;
        readonly IApplicationUsersService _applicationUsersService;
        readonly IReferentialsService _referentialsService;
        
        public ActionController(IPrepareService prepareService,
            IApplicationUsersService applicationUsersService,
            IReferentialsService referentialsService,
            ILocalizationManager localizationManager
            )
            : base(localizationManager)
        {
            _prepareService = prepareService;
            _applicationUsersService = applicationUsersService;
            _referentialsService = referentialsService;
        }

        /// <summary>
        /// Show more information about an action
        /// Routing create to be able to access as: /Action/Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Details(int? PublishModeFilter, int? id, bool computeReadNext=false, bool partial = false)
        {
            if (!id.HasValue)
                return HttpNotFound();

            var action = await _prepareService.GetPublishedAction(id.Value);
            if (action == null)
                return HttpNotFound();

            var publication = await _prepareService.GetLightPublication(action.Publication.PublicationId);

            ViewBag.ComputeReadNext = computeReadNext;
            ViewBag.publishModeFilter = PublishModeFilter.Value;

            // TODO : Faire le calcul du chemin critique en incluant les tâches des sous-process
            // Change to normal WBS order (alphanumeric)
            //if (computeReadNext)
            //{
            //    var tempCriticalPath = publication.PublishedActions.CriticalPath(p => p.Successors, p => p.BuildFinish - p.BuildStart);
            //    var criticalPath = new List<PublishedAction>();
            //    for (int i = tempCriticalPath.Count(); i > 0; i--)
            //        criticalPath.Add(tempCriticalPath.ElementAt(i - 1));

            //    var nextActionIndex = criticalPath.IndexOf(criticalPath.First(u => u.PublishedActionId == id)) + 1;
            //    ViewBag.NextAction = nextActionIndex >= criticalPath.Count ? 0 : criticalPath[nextActionIndex].PublishedActionId;

            //}
         

            var actionIndex = action.Publication.PublishedActions.OrderBy(a => a, new WBSComparer()).Select(p => p.PublishedActionId).ToList().IndexOf(action.PublishedActionId);
            var previousId = actionIndex - 1 >= 0 ? action.Publication.PublishedActions.OrderBy(a => a, new WBSComparer()).Select(p => p.PublishedActionId).ToList().ElementAtOrDefault(actionIndex - 1) : 0;
            var nextId = action.Publication.PublishedActions.OrderBy(a => a, new WBSComparer()).Select(p => p.PublishedActionId).ToList().ElementAtOrDefault(actionIndex + 1);

            var localizations = action.Publication.Localizations.ToDictionary(k => k.ResourceKey, v => v.Value);
            var documentationReferentials = await _referentialsService.GetDocumentationReferentials(action.Publication.ProcessId);
            localizations = ActionMapper.UpdateLocalizationLabelForDetail(localizations, documentationReferentials);
            var visibleColumns = ActionMapper.GetDetailDispositions(action.Publication, (PublishModeEnum)PublishModeFilter.Value);
            var values = new Dictionary<string, ActionColumnViewModel>();
            var actionHeaders = ActionMapper.GetDetailColumnHeader(visibleColumns, localizations);

            if (computeReadNext)
            {
                ViewBag.NextAction = nextId;
            }

            foreach (var setting in visibleColumns)
            {
                (List<RefsCollection> refs, List<CustomLabel> customLabel) = ActionMapper.BuildReferenceAndCustomLabel(action, localizations);
                var attribute = ReflectionHelper.GetPropertyValue(action, setting);
                values.Add(setting, ActionMapper.GetPublishedActionAttributes(action, attribute, setting, setting, localizations, refs, customLabel));
            }

            //Populate Action detail dispositions
            var detailsDisposition = new List<string>();
            if ((PublishModeEnum)PublishModeFilter.Value == PublishModeEnum.Formation)
            {
                if (publication.Formation_ActionDisposition.IsNotNullNorEmpty())
                {
                    detailsDisposition = publication.Formation_ActionDisposition.Split(',').ToList();
                }
            }
            else if ((PublishModeEnum)PublishModeFilter.Value == PublishModeEnum.Inspection)
            {
                if (publication.Inspection_ActionDisposition.IsNotNullNorEmpty())
                {
                    detailsDisposition = publication.Inspection_ActionDisposition.Split(',').ToList();
                }
            }
            
            

            var model = new GenericActionViewModel
            {
                ActionHeaders = actionHeaders,
                ActionId = action.PublishedActionId,
                Label = action.Label,
                ColumnValues = values,
                VideoExtension = action.CutVideo != null ? MimeMapping.GetMimeMapping(action.CutVideo.Extension) : null,
                VideoHash = action.CutVideo?.Hash,
                VideoExt = action.CutVideo?.Extension,
                ProcessId = action.Publication.ProcessId,
                ProcessLabel = action.Publication.Process.Label,
                PublicationVersion = action.Publication.Version,
                PublicationVersionIsMajor = action.Publication.IsMajor,
                PreviousActionId = previousId,
                NextActionId = nextId,
                IsKeyTask = action.IsKeyTask,
                PublishModeFilter = PublishModeFilter.Value,
                DetailActionDispositions = detailsDisposition
            };

            if (partial)
                return PartialView(model);

            return View(model);
        }
    }

}