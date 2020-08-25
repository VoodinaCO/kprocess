using KProcess.KL2.API.App_Start;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API.Controller
{
    [Authorize]
    [RoutePrefix("Services/ApplicationUsersService")]
    public class ApplicationUsersServiceController : ApiController, IApplicationUsersServiceController
    {
        private readonly ITraceManager _traceManager;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly ISecurityContext _securityContext;

        /// <summary>
        /// ApplicationUsersServiceController ctors
        /// </summary>
        /// <param name="applicationUsersService"></param>
        /// <param name="securityContext"></param>
        public ApplicationUsersServiceController(ITraceManager traceManager, IApplicationUsersService applicationUsersService, ISecurityContext securityContext)
        {
            _traceManager = traceManager;
            _applicationUsersService = applicationUsersService;
            _securityContext = securityContext;
        }


        /// <summary>
        /// Obtient les utilisateurs, les rôles et les langues disponibles.
        /// </summary>
        [HttpPost]
        [Route("GetUsersAndRolesAndLanguages")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetUsersAndRolesAndLanguages()
        {
            try
            {
                return Ok(await _applicationUsersService.GetUsersAndRolesAndLanguages());
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }


        /// <summary>
        /// Sauvegarde les utilisateurs spécifiés.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("SaveUsers")]
        public async Task<IHttpActionResult> SaveUsers([DynamicBody]dynamic param)
        {
            try
            {
                IEnumerable<Ksmed.Models.User> users = param.users;
                return Ok(await _applicationUsersService.SaveUsers(users));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("SaveTeams")]
        public async Task<IHttpActionResult> SaveTeams([DynamicBody]dynamic param)
        {
            try
            {
                IEnumerable<Ksmed.Models.Team> teams = param.teams;
                return Ok(await _applicationUsersService.SaveTeams(teams));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }
    }
}
