using KProcess;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Kprocess.KL2.FileServer
{
    public class MethodOverrideHandler : DelegatingHandler
    {
        readonly string[] _methods = { "BITS_POST" };
        const string _header = "X-HTTP-Method-Override";

        readonly ITraceManager _traceManager;

        public MethodOverrideHandler(ITraceManager traceManager)
        {
            _traceManager = traceManager;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Post && request.Headers.Contains(_header))
            {
                // Check if the header value is in our methods list.
                var method = request.Headers.GetValues(_header).First();
                if (_methods.Contains(method, StringComparer.InvariantCultureIgnoreCase))
                {
                    // Change the request method.
                    var oldMethod = new HttpMethod(request.Method.ToString());
                    request.Method = new HttpMethod(method);
                    _traceManager.TraceDebug($"Change HttpMethod {oldMethod} => {request.Method}");
                }
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}
