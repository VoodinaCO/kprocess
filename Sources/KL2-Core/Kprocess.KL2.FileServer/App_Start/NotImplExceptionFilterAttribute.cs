using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;

namespace Kprocess.KL2.FileServer.App_Start
{
    public class NotImplExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

            HttpError error = new HttpError
            {
                { nameof(Exception.Message) , "An error has occurred." },
                { nameof(Exception), JsonConvert.SerializeObject(actionExecutedContext.Exception, settings) }
            };


            try
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, error);
            }
            catch { }
        }
    }
}
