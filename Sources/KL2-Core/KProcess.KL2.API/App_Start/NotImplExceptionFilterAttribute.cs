using KProcess.Ksmed.Business;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;

namespace KProcess.KL2.API.App_Start
{
    public class NotImplExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

            // Quelques types d'exceptions doivent etre reecrits car il ne peuvent pas etre deserialize correctement
            switch (actionExecutedContext.Exception.GetType().ToString())
            {
                case "System.Data.Entity.Core.UpdateException":
                    actionExecutedContext.Exception = new BLLDalException(
                        "Impossible de mettre à jour la base de données.",
                        KnownErrorCodes.UpdateException);
                    break;
            }

            HttpError error = new HttpError
            {
                { "Message", "An error has occurred." },
                { "Exception", JsonConvert.SerializeObject(actionExecutedContext.Exception, settings) }
            };

            try
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, error);
            }
            catch { }
        }
    }
}
