using System.Threading.Tasks;
using TusDotNet.Adapters;
using TusDotNet.Constants;
using TusDotNet.Extensions;

namespace TusDotNet.Validation.Requirements
{
    internal sealed class RequestOffsetMatchesFileOffset : Requirement
    {
        public override async Task Validate(ContextAdapter context)
        {
            var requestOffset = long.Parse(context.Request.GetHeader(HeaderConstants.UploadOffset));
            var fileOffset =
                await context.Configuration.Store.GetUploadOffsetAsync(context.GetFileId(), context.CancellationToken);

            if (requestOffset != fileOffset)
            {
                await Conflict($"Offset does not match file. File offset: {fileOffset}. Request offset: {requestOffset}");
            }
        }
    }
}
