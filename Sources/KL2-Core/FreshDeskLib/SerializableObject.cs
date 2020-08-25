using Newtonsoft.Json;

namespace FreshDeskLib
{
    public abstract class SerializableObject
    {
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.None,
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
