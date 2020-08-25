using KProcess.KL2.API.App_Start;
using KProcess.KL2.API.Authentication;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API.Controller
{
    [SettingUserContextFilter]
    [RoutePrefix("Services/ReferentialsService")]
    public class ReferentialsServiceController : ApiController, IReferentialsServiceController
    {
        private readonly ITraceManager _traceManager;
        readonly IReferentialsService _referentialService;
        readonly ISecurityContext _securityContext;

        /// <summary>
        /// ReferentialServiceController ctors
        /// </summary>
        /// <param name="referentialService"></param>
        /// <param name="securityContext"></param>
        public ReferentialsServiceController(ITraceManager traceManager, IReferentialsService referentialService, ISecurityContext securityContext)
        {
            _traceManager = traceManager;
            _referentialService = referentialService;
            _securityContext = securityContext;
        }

        [HttpPost]
        [Route("GetLabel")]
        public IHttpActionResult GetLabel([DynamicBody]dynamic param)
        {
            try
            {
                ProcessReferentialIdentifier refe = (ProcessReferentialIdentifier)param.refe;
                var result = _referentialService.GetLabel(refe);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("GetLabelPlural")]
        public IHttpActionResult GetLabelPlural([DynamicBody]dynamic param)
        {
            try
            {
                ProcessReferentialIdentifier refe = (ProcessReferentialIdentifier)param.refe;
                var result = _referentialService.GetLabelPlural(refe);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient la configuration des réferentiels.
        /// </summary>
        [HttpPost]
        [Route("GetApplicationReferentials")]
        public async Task<IHttpActionResult> GetApplicationReferentials()
        {
            try
            {
                var result = await _referentialService.GetApplicationReferentials();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Met à jour le libellé du référentiel spécifié.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("UpdateReferentialLabel")]
        public async Task<IHttpActionResult> UpdateReferentialLabel([DynamicBody]dynamic param)
        {
            try
            {
                ProcessReferentialIdentifier refId = (ProcessReferentialIdentifier)param.refId;
                string label = param.label;
                await _referentialService.UpdateReferentialLabel(refId, label);
                return Ok();
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient dans l'ordre :
        /// Les catégories d'actions de tous les projets
        /// Tous les types d'actions 
        /// Toutes les valorisations d'actions.
        /// </summary>
        [HttpPost]
        [Route("LoadCategories")]
        public async Task<IHttpActionResult> LoadCategories()
        {
            try
            {
                var result = await _referentialService.LoadCategories();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }


        /// <summary>
        /// Sauvegarde les catégories.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("SaveCategories")]
        public async Task<IHttpActionResult> SaveCategories([DynamicBody]dynamic param)
        {
            try
            {
                IEnumerable<ActionCategory> categories = param.categories;
                var result = await _referentialService.SaveCategories(categories);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient les compétences d'actions de tous les projets
        /// </summary>
        [HttpPost]
        [Route("LoadSkills")]
        public async Task<IHttpActionResult> LoadSkills([DynamicBody]dynamic param)
        {
            try
            {
                bool allInfos = param.allInfos;
                var result = await _referentialService.LoadSkills(allInfos);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }


        /// <summary>
        /// Sauvegarde les compétences.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("SaveSkills")]
        public async Task<IHttpActionResult> SaveSkills([DynamicBody]dynamic param)
        {
            try
            {
                IEnumerable<Skill> skills = param.skills;
                var result = await _referentialService.SaveSkills(skills);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }



        /// <summary>
        /// Obtient les référentiels standards et projets du type spécifié.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("GetReferentials")]
        public async Task<IHttpActionResult> GetReferentials([DynamicBody]dynamic param)
        {
            try
            {
                ProcessReferentialIdentifier refId = (ProcessReferentialIdentifier)param.refId;
                int? processId = (int?)param.processId;
                var result = await _referentialService.GetReferentials(refId, processId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }


        /// <summary>
        /// Sauvegarde les référentiels.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("SaveReferentials")]
        public async Task<IHttpActionResult> SaveReferentials([DynamicBody]dynamic param)
        {
            try
            {
                var referentials = param.referentials;
                var result = await _referentialService.SaveReferentials(referentials);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }


        /// <summary>
        /// Obtient dans l'ordre :
        /// Les équipements de tous les projets.
        /// </summary>
        [HttpPost]
        [Route("LoadEquipments")]
        public async Task<IHttpActionResult> LoadEquipments()
        {
            try
            {
                var result = await _referentialService.LoadEquipments();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }


        /// <summary>
        /// Obtient dans l'ordre :
        /// Les opérateurs de tous les projets.
        /// </summary>
        [HttpPost]
        [Route("LoadOperators")]
        public async Task<IHttpActionResult> LoadOperators()
        {
            try
            {
                var result = await _referentialService.LoadOperators();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }


        /// <summary>
        /// Sauvegarde les ressources.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("SaveResources")]
        public async Task<IHttpActionResult> SaveResources([DynamicBody]dynamic param)
        {
            try
            {
                IEnumerable<Resource> resources = param.resources;
                var result = await _referentialService.SaveResources(resources);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
         }

        /// <summary>
        /// Fusionne des référentiels
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("MergeReferentials")]
        public async Task<IHttpActionResult> MergeReferentials([DynamicBody]dynamic param)
        {
            try
            {
                IActionReferential master = param.master;
                IActionReferential[] slaves = param.slaves;
                await _referentialService.MergeReferentials(master, slaves);
                return Ok();
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Check if a referential is used by documentation or analyst
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ReferentialUsed")]
        public async Task<IHttpActionResult> ReferentialUsed([DynamicBody]dynamic param)
        {
            try
            {
                ProcessReferentialIdentifier processReferentialId = (ProcessReferentialIdentifier)param.processReferentialId;
                int referentialId = (int)param.referentialId;
                var result = await _referentialService.ReferentialUsed(processReferentialId, referentialId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }
    }
}
