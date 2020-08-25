using System.Collections.Generic;

namespace FreshDeskLib
{
    public interface IAttachments
    {
        IDictionary<string, byte[]> Attachments { get; set; }
    }
}
