using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace KProcess.KL2.SignalRClient.Hub
{
    public abstract class BaseKL2Hub : Microsoft.AspNet.SignalR.Hub
    {
        bool _isConnected;
        string _groupName = string.Empty;

        readonly ConcurrentDictionary<string, ConcurrentQueue<string>> _dictionary = new ConcurrentDictionary<string, ConcurrentQueue<string>>();

        public override Task OnConnected()
        {
            _isConnected = true;
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            _isConnected = false;
            return base.OnDisconnected(stopCalled);
        }

        public bool IsAvailable()
        {
            return _isConnected;
        }

        public Task JoinGroup(string groupName)
        {
            _groupName = groupName;
            return Groups.Add(Context.ConnectionId, _groupName);
        }

        protected string[] GetConnectionId(string methodName)
        {
            _dictionary.TryRemove(methodName, out var queue);
            return queue?.ToArray();
        }

        protected void AddConnectionId(string methodName)
        {
            ConcurrentQueue<string> queue;
            if (!_dictionary.ContainsKey(methodName))
            {
                queue = new ConcurrentQueue<string>();
                queue.Enqueue(Context.ConnectionId);
                _dictionary.TryAdd(methodName, queue);
            }
            else
            {
                _dictionary.TryGetValue(methodName, out queue);
                queue?.Enqueue(Context.ConnectionId);
            }
        }
    }
}
