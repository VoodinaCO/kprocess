using System.Collections.Concurrent;

namespace KProcess.KL2.JWT
{
    public interface ITokenStore
    {
        ConcurrentBag<string> Tokens { get; }
    }
}
