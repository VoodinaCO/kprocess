using System.Collections.Generic;

namespace SyncfusionHelper
{
    public static class DtoConvertExtensions
    {
        public static SfDataGridXmlDto ToXmlDto(this SfDataGridJsonDto jsonDto)
        {
            var result = new SfDataGridXmlDto();

            foreach (var jsonColumn in jsonDto.Columns)
            {
                GridColumnXmlDto xmlColumn;
                if (jsonColumn.Type == "string")
                {
                    xmlColumn = new GridTextColumnXmlDto
                    {
                        MappingName = jsonColumn.MappingName,
                        HeaderText = jsonColumn.HeaderText,
                        AllowFiltering = jsonColumn.AllowFiltering,
                        HorizontalAlignment = jsonColumn.HorizontalAlignment,
                        Width = jsonColumn.Width,
                        IsHidden = !jsonColumn.IsVisible
                    };
                }
                else if (jsonColumn.Type == "number")
                {
                    xmlColumn = new GridNumericColumnXmlDto
                    {
                        MappingName = jsonColumn.MappingName,
                        HeaderText = jsonColumn.HeaderText,
                        AllowFiltering = jsonColumn.AllowFiltering,
                        HorizontalAlignment = jsonColumn.HorizontalAlignment,
                        Width = jsonColumn.Width,
                        IsHidden = !jsonColumn.IsVisible
                    };
                }
                else if (jsonColumn.Type == "checkbox")
                {
                    xmlColumn = new GridCheckBoxColumnXmlDto
                    {
                        MappingName = jsonColumn.MappingName,
                        HeaderText = jsonColumn.HeaderText,
                        AllowFiltering = jsonColumn.AllowFiltering,
                        HorizontalAlignment = jsonColumn.HorizontalAlignment,
                        Width = jsonColumn.Width,
                        IsHidden = !jsonColumn.IsVisible
                    };
                }
                else if (jsonColumn.Type == "date") //TODO
                {
                    xmlColumn = new GridTemplateColumnXmlDto
                    {
                        MappingName = jsonColumn.MappingName,
                        HeaderText = jsonColumn.HeaderText,
                        AllowFiltering = jsonColumn.AllowFiltering,
                        HorizontalAlignment = jsonColumn.HorizontalAlignment,
                        Width = jsonColumn.Width,
                        IsHidden = !jsonColumn.IsVisible
                    };
                }
                else if (jsonColumn.Type == "datetime") //TODO
                {
                    xmlColumn = new GridTemplateColumnXmlDto
                    {
                        MappingName = jsonColumn.MappingName,
                        HeaderText = jsonColumn.HeaderText,
                        AllowFiltering = jsonColumn.AllowFiltering,
                        HorizontalAlignment = jsonColumn.HorizontalAlignment,
                        Width = jsonColumn.Width,
                        IsHidden = !jsonColumn.IsVisible
                    };
                }
                else if (jsonColumn.Type == "boolean") //TODO
                {
                    xmlColumn = new GridTemplateColumnXmlDto
                    {
                        MappingName = jsonColumn.MappingName,
                        HeaderText = jsonColumn.HeaderText,
                        AllowFiltering = jsonColumn.AllowFiltering,
                        HorizontalAlignment = jsonColumn.HorizontalAlignment,
                        Width = jsonColumn.Width,
                        IsHidden = !jsonColumn.IsVisible
                    };
                }
                else if (jsonColumn.Type == "guid") //TODO
                {
                    xmlColumn = new GridTemplateColumnXmlDto
                    {
                        MappingName = jsonColumn.MappingName,
                        HeaderText = jsonColumn.HeaderText,
                        AllowFiltering = jsonColumn.AllowFiltering,
                        HorizontalAlignment = jsonColumn.HorizontalAlignment,
                        Width = jsonColumn.Width,
                        IsHidden = !jsonColumn.IsVisible
                    };
                }
                else
                {
                    xmlColumn = new GridTemplateColumnXmlDto
                    {
                        MappingName = jsonColumn.MappingName,
                        HeaderText = jsonColumn.HeaderText,
                        AllowFiltering = jsonColumn.AllowFiltering,
                        HorizontalAlignment = jsonColumn.HorizontalAlignment,
                        Width = jsonColumn.Width,
                        IsHidden = !jsonColumn.IsVisible
                    };
                }
                result.Columns.Add(xmlColumn);
            }

            return result;
        }

        public static SfDataGridJsonDto ToJsonDto(this SfDataGridXmlDto xmlDto, Dictionary<string, string> templates)
        {
            var result = new SfDataGridJsonDto();

            foreach (var xmlColumn in xmlDto.Columns)
            {
                GridColumnJsonDto jsonColumn = new GridColumnJsonDto
                {
                    MappingName = xmlColumn.MappingName,
                    HeaderText = xmlColumn.HeaderText,
                    AllowFiltering = xmlColumn.AllowFiltering,
                    Template = templates?.ContainsKey(xmlColumn.MappingName) == true ? templates[xmlColumn.MappingName] : "#NoTemplate",
                    HorizontalAlignment = xmlColumn.HorizontalAlignment,
                    Width = xmlColumn.Width,
                    IsVisible = !xmlColumn.IsHidden
                };
                result.Columns.Add(jsonColumn);
            }

            return result;
        }
    }
}
