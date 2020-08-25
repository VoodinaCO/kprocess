using System.Threading.Tasks;
using TusDotNet.Adapters;
using TusDotNet.Extensions;
using TusDotNet.Interfaces;

namespace TusDotNet.Validation.Requirements
{
    internal class FileHasNotExpired  : Requirement
    {
        public override async Task Validate(ContextAdapter context)
        {
            if (context.Configuration.Store is ITusExpirationStore expirationStore)
            {
                var expires = await expirationStore.GetExpirationAsync(context.GetFileId(), context.CancellationToken);
                if (expires?.HasPassed() == true)
                    await NotFound();
            }
        }
    }
}
