using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace KProcess.Ksmed.Server.Activation.KeygenApp.Freshdesk.Models
{
    class OutboundMail: SerializableObject
    {
        [JsonProperty("name")]
        public string RequesterName { get; set; }
        [JsonProperty("email")]
        public string RequesterEmail { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("status")]
        public int? Status { get; set; }
        [JsonProperty("priority")]
        public int? Priority { get; set; }
        [JsonProperty("description")]
        public string HtmlContent { get; set; }
        [JsonProperty("attachments")]
        public List<byte[]> Attachments { get; set; }
        [JsonProperty("email_config_id")]
        public long? EmailConfigId { get; set; }
        [JsonProperty("tags")]
        public List<string> Tags{ get; set; }

    }
}
