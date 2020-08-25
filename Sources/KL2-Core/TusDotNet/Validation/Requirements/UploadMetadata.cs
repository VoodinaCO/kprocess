using System.Threading.Tasks;
using TusDotNet.Adapters;
using TusDotNet.Constants;
using TusDotNet.Models;

namespace TusDotNet.Validation.Requirements
{
    internal sealed class UploadMetadata : Requirement
    {
        public override Task Validate(ContextAdapter context)
        {
            var request = context.Request;
            if (!request.Headers.ContainsKey(HeaderConstants.UploadMetadata))
            {
                return Done;
            }

            var validateMetadataResult = Metadata.ValidateMetadataHeader(request.Headers[HeaderConstants.UploadMetadata][0]);
            if (!string.IsNullOrEmpty(validateMetadataResult))
            {
                return BadRequest(validateMetadataResult);
            }

            return Done;
        }
    }
}
