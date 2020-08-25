using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace SyncfusionHelper
{
    public static class LinqXmlHelper
    {
        static readonly XNamespace XmlSyncfusionNameSpace = "http://schemas.datacontract.org/2004/07/Syncfusion.UI.Xaml.Grid";
        static readonly XNamespace XmlNameSpace = "http://www.w3.org/2001/XMLSchema-instance";
        static readonly XNamespace XmlPrimitivesNameSpace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
        static readonly XNamespace XmlSystemNameSpace = "http://schemas.datacontract.org/2004/07/System.Windows";

        public delegate bool TryParseHandler<T>(string value, out T result);
        public delegate T XmlParseHandler<T>(string value);

        public static readonly Dictionary<Type, Dictionary<string, Type>> TypesEquivalences = new Dictionary<Type, Dictionary<string, Type>>
        {
            [typeof(GridColumnXmlDto)] = new Dictionary<string, Type>
            {
                ["GridTextColumn"] = typeof(GridTextColumnXmlDto),
                ["GridNumericColumn"] = typeof(GridNumericColumnXmlDto),
                ["GridCheckBoxColumn"] = typeof(GridCheckBoxColumnXmlDto),
                ["GridTemplateColumn"] = typeof(GridTemplateColumnXmlDto)
            }
        };

        public static bool TryParse<T>(this XElement parent, string eltName, TryParseHandler<T> handler, out T value) where T : struct
        {
            value = default;
            XElement elt = parent?.Elements().FirstOrDefault(_ => _.Name == eltName);
            if (string.IsNullOrEmpty(elt?.Value))
                return false;
            if (handler(elt.Value, out T result))
            {
                value = result;
                return true;
            }
            return false;
        }

        public static bool TryParse<T>(this XElement parent, string eltName, XmlParseHandler<T> handler, out T value) where T : struct
        {
            value = default;
            XElement elt = parent?.Elements().FirstOrDefault(_ => _.Name == eltName);
            if (string.IsNullOrEmpty(elt?.Value))
                return false;
            try
            {
                value = handler(elt.Value);
                return true;
            }
            catch { }
            return false;
        }

        public static bool TryParseThickness(this XElement parent, string eltName, out Thickness result)
        {
            result = default;
            XElement elt = parent?.Elements().FirstOrDefault(_ => _.Name == eltName);
            if (elt == null)
                return false;
            double Bottom = 0;
            double Left = 0;
            double Right = 0;
            double Top = 0;
            if (elt.TryParse($"{{{XmlSystemNameSpace}}}{nameof(Thickness.Bottom)}", double.TryParse, out double doubleValue))
                Bottom = doubleValue;
            if (elt.TryParse($"{{{XmlSystemNameSpace}}}{nameof(Thickness.Left)}", double.TryParse, out doubleValue))
                Left = doubleValue;
            if (elt.TryParse($"{{{XmlSystemNameSpace}}}{nameof(Thickness.Right)}", double.TryParse, out doubleValue))
                Right = doubleValue;
            if (elt.TryParse($"{{{XmlSystemNameSpace}}}{nameof(Thickness.Top)}", double.TryParse, out doubleValue))
                Top = doubleValue;
            result = new Thickness(Left, Top, Right, Bottom);
            return true;
        }

        public static string ReadString(this XElement parent, string eltName)
        {
            var result = parent?.Elements().FirstOrDefault(_ => _.Name == eltName)?.Value;
            return string.IsNullOrEmpty(result) ? null : result;
        }

        public static TEnum? ReadEnum<TEnum>(this XElement parent, string eltName) where TEnum : struct
        {
            XElement elt = parent?.Elements().FirstOrDefault(_ => _.Name == eltName);
            if (string.IsNullOrEmpty(elt?.Value))
                return null;
            if (Enum.TryParse(elt.Value, true, out TEnum value))
                return value;
            return null;
        }

        public static T ReadXmlSerializableObject<T>(this XElement parent, string eltName) where T : IXmlDtoSerializable
        {
            var elt = parent?.Elements().FirstOrDefault(_ => _.Name == eltName);
            if (elt == null)
                return default(T);
            T result = Activator.CreateInstance<T>();
            result.Deserialize(elt);
            return result;
        }

        public static List<T> ReadCollection<T>(this XElement parent, string eltName) where T : class
        {
            XElement elt = parent?.Elements().FirstOrDefault(_ => _.Name == eltName);
            if (elt == null)
                return null;
            var result = new List<T>();
            foreach (var item in elt.Elements())
            {
                if (typeof(IXmlDtoSerializable).IsAssignableFrom(typeof(T)))
                {
                    object dItem = null;
                    if (TypesEquivalences.ContainsKey(typeof(T)))
                    {
                        string type = item.Attributes($"{{{XmlNameSpace}}}type").SingleOrDefault()?.Value;
                        if (string.IsNullOrEmpty(type))
                            continue;
                        if (TypesEquivalences[typeof(T)].ContainsKey(type))
                            dItem = Activator.CreateInstance(TypesEquivalences[typeof(T)][type]);
                        else
                            dItem = Activator.CreateInstance(typeof(T));
                    }
                    else
                        dItem = Activator.CreateInstance(typeof(T));
                    (dItem as IXmlDtoSerializable)?.Deserialize(item);
                    result.Add((T)dItem);
                }
            }
            return result;
        }

        public static List<int> ReadIntCollection(this XElement parent, string eltName)
        {
            XElement elt = parent?.Elements().FirstOrDefault(_ => _.Name == eltName);
            if (elt == null)
                return null;
            var result = new List<int>();
            foreach (var item in elt.Elements())
            {
                if (item.Name == $"{{{XmlPrimitivesNameSpace}}}int")
                {
                    if (string.IsNullOrEmpty(elt?.Value))
                        continue;
                    if (int.TryParse(elt.Value, out int intValue))
                        result.Add(intValue);
                }
            }
            return result;
        }

        public static XElement Write(this Thickness thickness, string eltName)
        {
            var result = new XElement(eltName,
                new XAttribute(XNamespace.Xmlns + "a", XmlSystemNameSpace));
            result.Add(new XElement($"{{{XmlSystemNameSpace}}}{nameof(Thickness.Bottom)}", thickness.Bottom));
            result.Add(new XElement($"{{{XmlSystemNameSpace}}}{nameof(Thickness.Left)}", thickness.Left));
            result.Add(new XElement($"{{{XmlSystemNameSpace}}}{nameof(Thickness.Right)}", thickness.Right));
            result.Add(new XElement($"{{{XmlSystemNameSpace}}}{nameof(Thickness.Top)}", thickness.Top));
            return result;
        }

        public static XElement Write(this List<int> list, string eltName)
        {
            var result = new XElement(eltName,
                new XAttribute(XNamespace.Xmlns + "a", XmlPrimitivesNameSpace));
            foreach (var value in list)
                result.Add(new XElement($"{{{XmlPrimitivesNameSpace}}}int", value));
            return result;
        }
    }
}
