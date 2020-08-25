using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace KProcess.KL2.SignalRClient.Context
{
    public abstract class BaseRespository<THub>
    {
        protected IHubContext _hub { get; set; }
        protected IHubConnectionContext<dynamic> Clients { get; set; }
        protected IGroupManager Groups { get; set; }

        protected BaseRespository(IConnectionManager connectionManager)
        {
            _hub = connectionManager.GetHubContext(typeof(THub).Name);
            Clients = _hub.Clients;
            Groups = _hub.Groups;
        }
    }
}
