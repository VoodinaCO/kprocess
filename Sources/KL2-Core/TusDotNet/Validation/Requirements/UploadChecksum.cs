using System.Linq;
using System.Threading.Tasks;
using TusDotNet.Adapters;
using TusDotNet.Constants;
using TusDotNet.Interfaces;
using TusDotNet.Models;

namespace TusDotNet.Validation.Requirements
{
    internal sealed class UploadChecksum : Requirement
    {
        public override async Task Validate(ContextAdapter context)
        {
            ITusChecksumStore checksumStore = context.Configuration.Store as ITusChecksumStore;
            var providedChecksum = GetProvidedChecksum(context);

            if (checksumStore != null && providedChecksum != null)
            {
                if (!providedChecksum.IsValid)
                {
                    await BadRequest($"Could not parse {HeaderConstants.UploadChecksum} header");
                    return;
                }

                var checksumAlgorithms = (await checksumStore.GetSupportedAlgorithmsAsync(context.CancellationToken)).ToList();
                if (!checksumAlgorithms.Contains(providedChecksum.Algorithm))
                {
                    await BadRequest(
                        $"Unsupported checksum algorithm. Supported algorithms are: {string.Join(",", checksumAlgorithms)}");
                }
            }
        }

        private static Checksum GetProvidedChecksum(ContextAdapter context)
        {
            return context.Request.Headers.ContainsKey(HeaderConstants.UploadChecksum)
                ? new Checksum(context.Request.Headers[HeaderConstants.UploadChecksum][0])
                : null;
        }
    }
}
