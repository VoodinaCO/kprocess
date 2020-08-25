using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace KProcess.KL2.WebAdmin.Serialization
{
    public class GenericModelBinder<T> : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            T model;

            if (controllerContext.RequestContext.HttpContext.Request.ContentType.ToLower() == "application/json"
                && (controllerContext.RequestContext.HttpContext.Request.AcceptTypes.Contains("application/json")
                    || controllerContext.RequestContext.HttpContext.Request.AcceptTypes.Contains("*/*")))
            {
                var jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, PreserveReferencesHandling = PreserveReferencesHandling.All };
                controllerContext.RequestContext.HttpContext.Request.InputStream.Seek(0, SeekOrigin.Begin);
                var json = new StreamReader(controllerContext.RequestContext.HttpContext.Request.InputStream).ReadToEnd();
                model = JsonConvert.DeserializeObject<T>(json, jsonSettings);
            }
            else
            {
                model = (T)ModelBinders.Binders.DefaultBinder.BindModel(controllerContext, bindingContext);
            }

            return model;
        }
    }
}