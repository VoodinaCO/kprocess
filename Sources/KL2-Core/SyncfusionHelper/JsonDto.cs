using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows;

namespace SyncfusionHelper
{
    public class SfDataGridJsonDto
    {
        [JsonProperty("cols")]
        public List<GridColumnJsonDto> Columns { get; set; } = new List<GridColumnJsonDto>();
    }

    public class GridColumnJsonDto
    {
        [JsonProperty("field")]
        public string MappingName { get; set; } = null;

        [JsonProperty("headerText")]
        public string HeaderText { get; set; } = null;

        [JsonProperty("allowFiltering")]
        public bool AllowFiltering { get; set; } = true;

        [JsonProperty("template")]
        public string Template { get; set; }

        [JsonProperty("textAlign")]
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Left;

        [JsonProperty("width")]
        public string WidthString { get; set; }

        [JsonIgnore]
        public double Width
        {
            get
            {
                if (double.TryParse(WidthString, out double result))
                    return result;
                else
                    return double.NaN;
            }
            set
            {
                WidthString = value.ToString();
            }
        }

        [JsonProperty("visible")]
        public bool IsVisible { get; set; } = false;

        /// <summary>
        /// https://help.syncfusion.com/aspnetmvc/grid/columns#type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
