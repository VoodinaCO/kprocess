using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using DlhSoft.Windows.Data;

namespace DlhSoft.Windows.Data
{
    // Ajout Tekigo
    public class TimeOfDayConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string input = ((string)value).Trim();
                try
                {
                    return new TimeOfDay(TimeSpan.Parse(input, culture));
                }
                catch (FormatException exception)
                {
                    throw new FormatException("ConvertInvalidPrimitive", exception);
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if (destinationType == typeof(InstanceDescriptor) && (value is TimeOfDay))
            {
                MethodInfo method = typeof(TimeSpan).GetMethod("Parse", new Type[] { typeof(string) });
                if (method != null)
                {
                    return new InstanceDescriptor(method, new object[] { ((TimeOfDay)value).ToTimeSpan().ToString() });
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
