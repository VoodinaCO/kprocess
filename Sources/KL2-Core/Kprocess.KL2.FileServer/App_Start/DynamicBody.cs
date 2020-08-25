using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;

namespace Kprocess.KL2.FileServer.App_Start
{
    /// <summary>
    /// An attribute that captures the entire content body and stores it
    /// into the parameter of type string or byte[].
    /// </summary>
    /// <remarks>
    /// The parameter marked up with this attribute should be the only parameter as it reads the
    /// entire request body and assigns it to that parameter.    
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class DynamicBodyAttribute : ParameterBindingAttribute
    {
        public override HttpParameterBinding GetBinding(HttpParameterDescriptor parameter)
        {
            if (parameter == null)
                throw new ArgumentException("Invalid parameter");

            return new DynamicBodyParameterBinding(parameter);
        }
    }

    /// <summary>
    /// Reads the Request body into a string/byte[] and
    /// assigns it to the parameter bound.
    /// 
    /// Should only be used with a single parameter on
    /// a Web API method using the [DynamicBody] attribute
    /// </summary>
    public class DynamicBodyParameterBinding : HttpParameterBinding
    {
        public DynamicBodyParameterBinding(HttpParameterDescriptor descriptor)
            : base(descriptor)
        {
        }

        public override async Task ExecuteBindingAsync(ModelMetadataProvider metadataProvider, HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            HttpActionBinding binding = actionContext.ActionDescriptor.ActionBinding;

            if (binding.ParameterBindings.Length > 1 || actionContext.Request.Method == HttpMethod.Get)
                return;

            string stringResult = await actionContext.Request.Content.ReadAsStringAsync();
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            dynamic param = JsonConvert.DeserializeObject(stringResult, settings);
            SetValue(actionContext, param);
        }

        public override bool WillReadBody =>
            true;
    }
}
