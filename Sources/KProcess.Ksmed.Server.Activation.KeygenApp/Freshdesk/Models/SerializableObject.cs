using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Server.Activation.KeygenApp.Freshdesk.Models
{
    public class SerializableObject
    {
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

        }
        public bool ShouldSerializeId()
        {
            return false;
        }
    }
}
