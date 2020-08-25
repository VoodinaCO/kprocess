using System;
using System.Threading.Tasks;
using TusDotNet.Adapters;

namespace TusDotNet.Validation.Requirements
{
    internal sealed class ContentType : Requirement
    {
        public override Task Validate(ContextAdapter context)
        {
            if (context.Request.ContentType?.Equals("application/offset+octet-stream", StringComparison.OrdinalIgnoreCase) != true)
            {
                return BadRequest($"Content-Type {context.Request.ContentType} is invalid. Must be application/offset+octet-stream");
            }

            return Done;
        }
    }
}
