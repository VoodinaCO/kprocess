using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Kprocess.KL2.FileServer.Serialization
{
    public class ValueTupleConverter<T1, T2> : TypeConverter
        where T1 : struct where T2 : struct
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var elements = ((string)value).Split(new[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);

            return (JsonConvert.DeserializeObject<T1>(elements.First()), JsonConvert.DeserializeObject<T2>(elements.Last()));
        }
    }
}
