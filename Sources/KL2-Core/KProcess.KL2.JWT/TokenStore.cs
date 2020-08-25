using System.Collections.Concurrent;

namespace KProcess.KL2.JWT
{
    public class TokenStore : ITokenStore
    {
        public ConcurrentBag<string> Tokens { get; } = new ConcurrentBag<string>();
    }
}
