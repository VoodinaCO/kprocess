using System.Threading.Tasks;
using TusDotNet.Adapters;
using TusDotNet.Extensions;

namespace TusDotNet.Validation.Requirements
{
    internal sealed class FileExist : Requirement
    {
        public override async Task Validate(ContextAdapter context)
        {
            var exists = await context.Configuration.Store.FileExistAsync(context.GetFileId(), context.CancellationToken);
            if (!exists)
            {
                await NotFound();
            }
        }
    }
}
