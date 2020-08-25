using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Server.Activation.KeygenApp.Freshdesk.Models
{
    class Attachment
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("content_type")]        
        public string ContentType { get; set; }
        [JsonProperty("size")]
        public string Size { get; set; }
    }
}
