using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;

namespace SyncfusionHelper
{
    public interface IXmlDtoSerializable
    {
        XNamespace NameSpace { get; }
        string LocalName { get; }
        string XName { get; }
        void Deserialize(XElement root);
        XElement Serialize();
    }

    public class SfDataGridXmlDto : IXmlDtoSerializable
    {
        public bool AllowDraggingColumns { get; set; } = true;

        public bool AllowGrouping { get; set; } = true;

        public bool AllowResizingColumns { get; set; } = true;

        public GridLengthUnitType? ColumnSizer { get; set; } = null;

        public List<GridColumnXmlDto> Columns { get; set; } = new List<GridColumnXmlDto>();

        public Thickness CurrentCellBorderThickness { get; set; } = new Thickness(2);

        public int DataFetchSize { get; set; } = 5;

        public DetailsViewDefinitionXmlDto DetailsViewDefinition { get; set; } = new DetailsViewDefinitionXmlDto();

        public Thickness DetailsViewPadding { get; set; } = new Thickness(0, 10, 10, 10);

        public EditTrigger EditTrigger { get; set; } = EditTrigger.OnDoubleTap;

        public double ExpanderColumnWidth { get; set; } = 24;

        public string FilterSettings { get; set; } = string.Empty;

        public GridCopyOption GridCopyOption { get; set; } = GridCopyOption.CopyData;

        public GridPasteOption GridPasteOption { get; set; } = GridPasteOption.PasteData;

        public GridValidationMode GridValidationMode { get; set; } = GridValidationMode.None;

        public List<GroupColumnDescriptionXmlDto> GroupColumnDescriptions { get; set; }

        public string GroupDropAreaText { get; set; } = "Drag a column header here to group by that column";

        public List<GridSummaryRowXmlDto> GroupSummaryRows { get; set; }

        public double HeaderRowHeight { get; set; } = 35;

        public double IndentColumnWidth { get; set; } = 24;

        public LiveDataUpdateMode LiveDataUpdateMode { get; set; } = LiveDataUpdateMode.AllowDataShaping;

        public bool ReuseRowsOnItemssourceChange { get; set; } = true;

        public double RowHeaderWidth { get; set; } = 24;

        public double RowHeight { get; set; } = 178;

        public GridSelectionMode SelectionMode { get; set; } = GridSelectionMode.Single;

        public bool ShowColumnWhenGrouped { get; set; } = true;

        public List<SortColumnDescriptionXmlDto> SortColumnDescriptions { get; set; } = new List<SortColumnDescriptionXmlDto>(new []{ new SortColumnDescriptionXmlDto { ColumnName = "WBS" } });

        public List<StackedHeaderRowXmlDto> StackedHeaderRows { get; set; }

        public List<GridSummaryRowXmlDto> TableSummaryRows { get; set; }

        public List<UnBoundRowXmlDto> UnBoundRows { get; set; }

        public XNamespace NameSpace =>
            "http://schemas.datacontract.org/2004/07/Syncfusion.UI.Xaml.Grid";

        public virtual string LocalName =>
            nameof(SfDataGrid);

        public string XName =>
            $"{{{NameSpace}}}{LocalName}";

        public void Deserialize(XElement root)
        {
            if (root.Name != XName)
                return;

            if (root.TryParse($"{{{NameSpace}}}{nameof(AllowDraggingColumns)}", bool.TryParse, out bool boolValue))
                AllowDraggingColumns = boolValue;
            if (root.TryParse($"{{{NameSpace}}}{nameof(AllowGrouping)}", bool.TryParse, out boolValue))
                AllowGrouping = boolValue;
            if (root.TryParse($"{{{NameSpace}}}{nameof(AllowResizingColumns)}", bool.TryParse, out boolValue))
                AllowResizingColumns = boolValue;
            ColumnSizer = root.ReadEnum<GridLengthUnitType>($"{{{NameSpace}}}{nameof(ColumnSizer)}");
            Columns = root.ReadCollection<GridColumnXmlDto>($"{{{NameSpace}}}{nameof(Columns)}");
            if (root.TryParseThickness($"{{{NameSpace}}}{nameof(CurrentCellBorderThickness)}", out Thickness thicknessValue))
                CurrentCellBorderThickness = thicknessValue;
            if (root.TryParse($"{{{NameSpace}}}{nameof(DataFetchSize)}", int.TryParse, out int intValue))
                DataFetchSize = intValue;
            DetailsViewDefinition = root.ReadXmlSerializableObject<DetailsViewDefinitionXmlDto>($"{{{NameSpace}}}{nameof(DetailsViewDefinition)}");
            if (root.TryParseThickness($"{{{NameSpace}}}{nameof(DetailsViewPadding)}", out thicknessValue))
                DetailsViewPadding = thicknessValue;
            EditTrigger = root.ReadEnum<EditTrigger>($"{{{NameSpace}}}{nameof(EditTrigger)}") ?? EditTrigger.OnDoubleTap;
            if (root.TryParse($"{{{NameSpace}}}{nameof(ExpanderColumnWidth)}", XmlConvert.ToDouble, out double doubleValue))
                ExpanderColumnWidth = doubleValue;
            FilterSettings = root.ReadString($"{{{NameSpace}}}{nameof(FilterSettings)}");
            GridCopyOption = root.ReadEnum<GridCopyOption>($"{{{NameSpace}}}{nameof(GridCopyOption)}") ?? GridCopyOption.CopyData;
            GridPasteOption = root.ReadEnum<GridPasteOption>($"{{{NameSpace}}}{nameof(GridPasteOption)}") ?? GridPasteOption.PasteData;
            GridValidationMode = root.ReadEnum<GridValidationMode>($"{{{NameSpace}}}{nameof(GridValidationMode)}") ?? GridValidationMode.None;
            //GroupColumnDescriptions = root.ReadCollection<GroupColumnDescriptionXmlDto>($"{{{NameSpace}}}{nameof(GroupColumnDescriptions)}");
            GroupColumnDescriptions = new List<GroupColumnDescriptionXmlDto>();
            GroupDropAreaText = root.ReadString($"{{{NameSpace}}}{nameof(GroupDropAreaText)}");
            //GroupSummaryRows = root.ReadCollection<GridSummaryRowXmlDto>($"{{{NameSpace}}}{nameof(GroupSummaryRows)}");
            GroupSummaryRows = new List<GridSummaryRowXmlDto>();
            if (root.TryParse($"{{{NameSpace}}}{nameof(HeaderRowHeight)}", XmlConvert.ToDouble, out doubleValue))
                HeaderRowHeight = doubleValue;
            if (root.TryParse($"{{{NameSpace}}}{nameof(IndentColumnWidth)}", XmlConvert.ToDouble, out doubleValue))
                IndentColumnWidth = doubleValue;
            LiveDataUpdateMode = root.ReadEnum<LiveDataUpdateMode>($"{{{NameSpace}}}{nameof(LiveDataUpdateMode)}") ?? LiveDataUpdateMode.AllowDataShaping;
            if (root.TryParse($"{{{NameSpace}}}{nameof(ReuseRowsOnItemssourceChange)}", bool.TryParse, out boolValue))
                ReuseRowsOnItemssourceChange = boolValue;
            if (root.TryParse($"{{{NameSpace}}}{nameof(RowHeaderWidth)}", XmlConvert.ToDouble, out doubleValue))
                RowHeaderWidth = doubleValue;
            if (root.TryParse($"{{{NameSpace}}}{nameof(RowHeight)}", XmlConvert.ToDouble, out doubleValue))
                RowHeight = doubleValue;
            SelectionMode = root.ReadEnum<GridSelectionMode>($"{{{NameSpace}}}{nameof(SelectionMode)}") ?? GridSelectionMode.Single;
            if (root.TryParse($"{{{NameSpace}}}{nameof(ShowColumnWhenGrouped)}", bool.TryParse, out boolValue))
                ShowColumnWhenGrouped = boolValue;
            SortColumnDescriptions = root.ReadCollection<SortColumnDescriptionXmlDto>($"{{{NameSpace}}}{nameof(SortColumnDescriptions)}");
            //StackedHeaderRows = root.ReadCollection<StackedHeaderRowXmlDto>($"{{{NameSpace}}}{nameof(StackedHeaderRows)}");
            //TableSummaryRows = root.ReadCollection<GridSummaryRowXmlDto>($"{{{NameSpace}}}{nameof(TableSummaryRows)}");
            //UnBoundRows = root.ReadCollection<UnBoundRowXmlDto>($"{{{NameSpace}}}{nameof(UnBoundRows)}");
            StackedHeaderRows = new List<StackedHeaderRowXmlDto>();
            TableSummaryRows = new List<GridSummaryRowXmlDto>();
            UnBoundRows = new List<UnBoundRowXmlDto>();
        }

        public virtual XElement Serialize()
        {
            var result = new XElement(XName,
                new XAttribute("xmlns", "http://schemas.datacontract.org/2004/07/Syncfusion.UI.Xaml.Grid"),
                new XAttribute(XNamespace.Xmlns + "i", "http://www.w3.org/2001/XMLSchema-instance"));

            result.Add(new XElement($"{{{NameSpace}}}{nameof(AllowDraggingColumns)}", XmlConvert.ToString(AllowDraggingColumns)));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(AllowGrouping)}", XmlConvert.ToString(AllowGrouping)));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(AllowResizingColumns)}", XmlConvert.ToString(AllowResizingColumns)));
            if (ColumnSizer != null)
                result.Add(new XElement($"{{{NameSpace}}}{nameof(ColumnSizer)}", $"{ColumnSizer}"));
            var columns = new XElement($"{{{NameSpace}}}{nameof(Columns)}");
            foreach (var column in Columns)
                columns.Add(column.Serialize());
            result.Add(columns);
            result.Add(CurrentCellBorderThickness.Write($"{{{NameSpace}}}{nameof(CurrentCellBorderThickness)}"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(DataFetchSize)}", $"{DataFetchSize}"));
            if (DetailsViewDefinition != null)
                result.Add(DetailsViewDefinition.Serialize());
            result.Add(DetailsViewPadding.Write($"{{{NameSpace}}}{nameof(DetailsViewPadding)}"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(EditTrigger)}", $"{EditTrigger}"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(ExpanderColumnWidth)}", $"{ExpanderColumnWidth}"));
            if (FilterSettings == null)
                result.Add(new XElement($"{{{NameSpace}}}{nameof(FilterSettings)}"));
            else
                result.Add(new XElement($"{{{NameSpace}}}{nameof(FilterSettings)}", FilterSettings));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(GridCopyOption)}", $"{GridCopyOption}"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(GridPasteOption)}", $"{GridPasteOption}"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(GridValidationMode)}", $"{GridValidationMode}"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(GroupColumnDescriptions)}"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(GroupDropAreaText)}", $"{GroupDropAreaText}"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(GroupSummaryRows)}"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(HeaderRowHeight)}", $"{HeaderRowHeight}"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(IndentColumnWidth)}", $"{IndentColumnWidth}"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(LiveDataUpdateMode)}", $"{LiveDataUpdateMode}"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(ReuseRowsOnItemssourceChange)}", XmlConvert.ToString(ReuseRowsOnItemssourceChange)));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(RowHeaderWidth)}", $"{RowHeaderWidth}"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(RowHeight)}", $"{RowHeight}"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(SelectionMode)}", $"{SelectionMode}"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(ShowColumnWhenGrouped)}", XmlConvert.ToString(ShowColumnWhenGrouped)));
            if (SortColumnDescriptions != null)
            {
                var sortColumnDescriptions = new XElement($"{{{NameSpace}}}{nameof(SortColumnDescriptions)}");
                foreach (var sortColumnDescription in SortColumnDescriptions)
                    sortColumnDescriptions.Add(sortColumnDescription.Serialize());
                result.Add(sortColumnDescriptions);
            }
            result.Add(new XElement($"{{{NameSpace}}}{nameof(StackedHeaderRows)}"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(TableSummaryRows)}"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(UnBoundRows)}"));

            return result;
        }
    }

    public class DataGridXmlDto : SfDataGridXmlDto
    {
        public override string LocalName =>
            nameof(DataGrid);

        public override XElement Serialize()
        {
            var result = base.Serialize();

            result.Name = XName;
            result.Attributes().Remove();

            if (AllowDraggingColumns)
                result.Elements().FirstOrDefault(_ => _.Name == $"{{{NameSpace}}}{nameof(AllowDraggingColumns)}")?.Remove();
            if (AllowResizingColumns)
                result.Elements().FirstOrDefault(_ => _.Name == $"{{{NameSpace}}}{nameof(AllowResizingColumns)}")?.Remove();

            return result;
        }
    }

    public abstract class GridColumnXmlDto : IXmlDtoSerializable
    {
        public bool AllowBlankFilters { get; set; } = true;

        public bool AllowEditing { get; set; } = true;

        public bool AllowFiltering { get; set; } = true;

        public bool AllowFocus { get; set; } = true;

        public bool AllowGrouping { get; set; } = true;

        public bool AllowResizing { get; set; } = true;

        public bool AllowSorting { get; set; } = true;

        public GridLengthUnitType? ColumnSizer { get; set; } = null;

        public string FilterRowCondition { get; set; } = "BeginsWith";

        public string FilterRowEditorType { get; set; }

        public string FilteredFrom { get; set; } = "None";

        public GridValidationMode GridValidationMode { get; set; } = GridValidationMode.None;

        public string HeaderText { get; set; } = null;

        public HorizontalAlignment HorizontalHeaderContentAlignment { get; set; } = HorizontalAlignment.Left;

        public bool IsHidden { get; set; } = false;

        public string MappingName { get; set; } = null;

        public double MaximumWidth { get; set; } = double.NaN;

        public double MinimumWidth { get; set; } = double.NaN;

        public double Width { get; set; } = double.NaN;

        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Stretch;

        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Center;

        public XNamespace NameSpace =>
            "http://schemas.datacontract.org/2004/07/Syncfusion.UI.Xaml.Grid";

        public virtual string LocalName =>
            "GridColumn";

        public string XName =>
            $"{{{NameSpace}}}{LocalName}";

        public virtual void Deserialize(XElement root)
        {
            if (root.Name != XName)
                return;

            if (root.TryParse($"{{{NameSpace}}}{nameof(AllowBlankFilters)}", bool.TryParse, out bool boolValue))
                AllowBlankFilters = boolValue;
            if (root.TryParse($"{{{NameSpace}}}{nameof(AllowEditing)}", bool.TryParse, out boolValue))
                AllowEditing = boolValue;
            if (root.TryParse($"{{{NameSpace}}}{nameof(AllowFiltering)}", bool.TryParse, out boolValue))
                AllowFiltering = boolValue;
            if (root.TryParse($"{{{NameSpace}}}{nameof(AllowFocus)}", bool.TryParse, out boolValue))
                AllowFocus = boolValue;
            if (root.TryParse($"{{{NameSpace}}}{nameof(AllowGrouping)}", bool.TryParse, out boolValue))
                AllowGrouping = boolValue;
            if (root.TryParse($"{{{NameSpace}}}{nameof(AllowResizing)}", bool.TryParse, out boolValue))
                AllowResizing = boolValue;
            if (root.TryParse($"{{{NameSpace}}}{nameof(AllowSorting)}", bool.TryParse, out boolValue))
                AllowSorting = boolValue;
            ColumnSizer = root.ReadEnum<GridLengthUnitType>($"{{{NameSpace}}}{nameof(ColumnSizer)}");
            if (root.ReadString($"{{{NameSpace}}}{nameof(FilterRowCondition)}") != null)
                FilterRowCondition = root.ReadString($"{{{NameSpace}}}{nameof(FilterRowCondition)}");
            if (root.ReadString($"{{{NameSpace}}}{nameof(FilterRowEditorType)}") != null)
                FilterRowEditorType = root.ReadString($"{{{NameSpace}}}{nameof(FilterRowEditorType)}");
            if (root.ReadString($"{{{NameSpace}}}{nameof(FilteredFrom)}") != null)
                FilteredFrom = root.ReadString($"{{{NameSpace}}}{nameof(FilteredFrom)}");
            if (root.ReadEnum<GridValidationMode>($"{{{NameSpace}}}{nameof(GridValidationMode)}") != null)
                GridValidationMode = root.ReadEnum<GridValidationMode>($"{{{NameSpace}}}{nameof(GridValidationMode)}").Value;
            HeaderText = root.ReadString($"{{{NameSpace}}}{nameof(HeaderText)}");
            if (root.ReadEnum<HorizontalAlignment>($"{{{NameSpace}}}{nameof(HorizontalHeaderContentAlignment)}") != null)
                HorizontalHeaderContentAlignment = root.ReadEnum<HorizontalAlignment>($"{{{NameSpace}}}{nameof(HorizontalHeaderContentAlignment)}").Value;
            if (root.TryParse($"{{{NameSpace}}}{nameof(IsHidden)}", bool.TryParse, out boolValue))
                IsHidden = boolValue;
            MappingName = root.ReadString($"{{{NameSpace}}}{nameof(MappingName)}");
            if (root.TryParse($"{{{NameSpace}}}{nameof(MaximumWidth)}", XmlConvert.ToDouble, out double doubleValue))
                MaximumWidth = doubleValue;
            if (root.TryParse($"{{{NameSpace}}}{nameof(MinimumWidth)}", XmlConvert.ToDouble, out doubleValue))
                MinimumWidth = doubleValue;
            if (root.TryParse($"{{{NameSpace}}}{nameof(Width)}", XmlConvert.ToDouble, out doubleValue))
                Width = doubleValue;
            if (root.ReadEnum<HorizontalAlignment>($"{{{NameSpace}}}{nameof(HorizontalAlignment)}") != null)
                HorizontalAlignment = root.ReadEnum<HorizontalAlignment>($"{{{NameSpace}}}{nameof(HorizontalAlignment)}").Value;
            if (root.ReadEnum<VerticalAlignment>($"{{{NameSpace}}}{nameof(VerticalAlignment)}") != null)
                VerticalAlignment = root.ReadEnum<VerticalAlignment>($"{{{NameSpace}}}{nameof(VerticalAlignment)}").Value;
        }

        public virtual XElement Serialize()
        {
            var result = new XElement(XName);

            result.Add(new XElement($"{{{NameSpace}}}{nameof(AllowBlankFilters)}", XmlConvert.ToString(AllowBlankFilters)));
            if (!AllowEditing)
                result.Add(new XElement($"{{{NameSpace}}}{nameof(AllowEditing)}", XmlConvert.ToString(AllowEditing)));
            if (!AllowFiltering)
                result.Add(new XElement($"{{{NameSpace}}}{nameof(AllowFiltering)}", XmlConvert.ToString(AllowFiltering)));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(AllowFocus)}", XmlConvert.ToString(AllowFocus)));
            if (!AllowGrouping)
                result.Add(new XElement($"{{{NameSpace}}}{nameof(AllowGrouping)}", XmlConvert.ToString(AllowGrouping)));
            if (!AllowResizing)
                result.Add(new XElement($"{{{NameSpace}}}{nameof(AllowResizing)}", XmlConvert.ToString(AllowResizing)));
            if (!AllowSorting)
                result.Add(new XElement($"{{{NameSpace}}}{nameof(AllowSorting)}", XmlConvert.ToString(AllowSorting)));
            if (ColumnSizer != null)
                result.Add(new XElement($"{{{NameSpace}}}{nameof(ColumnSizer)}", $"{ColumnSizer}"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(FilterRowCondition)}", FilterRowCondition));
            if (FilterRowEditorType == null)
                result.Add(new XElement($"{{{NameSpace}}}{nameof(FilterRowEditorType)}"));
            else
                result.Add(new XElement($"{{{NameSpace}}}{nameof(FilterRowEditorType)}", FilterRowEditorType));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(FilteredFrom)}", FilteredFrom));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(GridValidationMode)}", $"{GridValidationMode}"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(HeaderText)}", HeaderText));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(HorizontalHeaderContentAlignment)}", $"{HorizontalHeaderContentAlignment}"));
            if (IsHidden)
                result.Add(new XElement($"{{{NameSpace}}}{nameof(IsHidden)}", XmlConvert.ToString(IsHidden)));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(MappingName)}", MappingName));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(MaximumWidth)}", MaximumWidth));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(MinimumWidth)}", MinimumWidth));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(Width)}", Width));

            return result;
        }
    }

    public class GridTextColumnXmlDto : GridColumnXmlDto
    {
        public TextWrapping TextWrapping { get; set; } = TextWrapping.NoWrap;

        public GridTextColumnXmlDto()
        {
            HorizontalAlignment = HorizontalAlignment.Left;
        }

        public override void Deserialize(XElement root)
        {
            if (root.Name != XName)
                return;

            base.Deserialize(root);

            if (root.ReadEnum<TextWrapping>($"{{{NameSpace}}}{nameof(TextWrapping)}") != null)
                TextWrapping = root.ReadEnum<TextWrapping>($"{{{NameSpace}}}{nameof(TextWrapping)}").Value;
        }

        public override XElement Serialize()
        {
            var result = base.Serialize();
            result.Add(new XAttribute("{http://www.w3.org/2001/XMLSchema-instance}type", "GridTextColumn"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(TextWrapping)}", $"{TextWrapping}"));

            return result;
        }
    }

    public class GridNumericColumnXmlDto : GridColumnXmlDto
    {
        public HorizontalAlignment TextAlignment { get; set; } = HorizontalAlignment.Right;

        public decimal MaxValue { get; set; } = decimal.MaxValue;

        public decimal MinValue { get; set; } = decimal.MinValue;

        public string NullText { get; set; }

        public int NumberDecimalDigits { get; set; } = 2;

        public string NumberDecimalSeparator { get; set; } = ",";

        public string NumberGroupSeparator { get; set; } = " ";

        public List<int> NumberGroupSizes { get; set; } = new List<int>{ 0 };

        public int NumberNegativePattern { get; set; } = 1;

        public GridNumericColumnXmlDto()
        {
            FilterRowCondition = "Equals";
            HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        public override void Deserialize(XElement root)
        {
            if (root.Name != XName)
                return;

            base.Deserialize(root);

            if (root.ReadEnum<HorizontalAlignment>($"{{{NameSpace}}}{nameof(TextAlignment)}") != null)
                TextAlignment = root.ReadEnum<HorizontalAlignment>($"{{{NameSpace}}}{nameof(TextAlignment)}").Value;
            if (root.TryParse($"{{{NameSpace}}}{nameof(MaxValue)}", decimal.TryParse, out decimal decimalValue))
                MaxValue = decimalValue;
            if (root.TryParse($"{{{NameSpace}}}{nameof(MinValue)}", decimal.TryParse, out decimalValue))
                MinValue = decimalValue;
            if (root.ReadString($"{{{NameSpace}}}{nameof(NullText)}") != null)
                NullText = root.ReadString($"{{{NameSpace}}}{nameof(NullText)}");
            if (root.TryParse($"{{{NameSpace}}}{nameof(NumberDecimalDigits)}", int.TryParse, out int intValue))
                NumberDecimalDigits = intValue;
            if (root.ReadString($"{{{NameSpace}}}{nameof(NumberDecimalSeparator)}") != null)
                NumberDecimalSeparator = root.ReadString($"{{{NameSpace}}}{nameof(NumberDecimalSeparator)}");
            if (root.ReadString($"{{{NameSpace}}}{nameof(NumberGroupSeparator)}") != null)
                NumberGroupSeparator = root.ReadString($"{{{NameSpace}}}{nameof(NumberGroupSeparator)}");
            NumberGroupSizes = root.ReadIntCollection($"{{{NameSpace}}}{nameof(NumberGroupSizes)}");
            if (root.TryParse($"{{{NameSpace}}}{nameof(NumberNegativePattern)}", int.TryParse, out intValue))
                NumberNegativePattern = intValue;
        }

        public override XElement Serialize()
        {
            var result = base.Serialize();
            result.Add(new XAttribute("{http://www.w3.org/2001/XMLSchema-instance}type", "GridNumericColumn"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(MaxValue)}", MaxValue));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(MinValue)}", MinValue));
            if (NullText == null)
                result.Add(new XElement($"{{{NameSpace}}}{nameof(NullText)}"));
            else
                result.Add(new XElement($"{{{NameSpace}}}{nameof(NullText)}", NullText));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(NumberDecimalDigits)}", NumberDecimalDigits));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(NumberDecimalSeparator)}", NumberDecimalSeparator));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(NumberGroupSeparator)}", NumberGroupSeparator));
            result.Add(NumberGroupSizes.Write($"{{{NameSpace}}}{nameof(NumberGroupSizes)}"));
            result.Add(new XElement($"{{{NameSpace}}}{nameof(NumberNegativePattern)}", NumberNegativePattern));

            var widthNode = result.Element($"{{{NameSpace}}}{nameof(Width)}");
            widthNode.AddBeforeSelf(new XElement($"{{{NameSpace}}}{nameof(TextAlignment)}", $"{TextAlignment}"));
            var afterList = new Stack<XElement>();
            if (HorizontalAlignment != HorizontalAlignment.Stretch)
                afterList.Push(new XElement($"{{{NameSpace}}}{nameof(HorizontalAlignment)}", $"{HorizontalAlignment}"));
            if (VerticalAlignment != VerticalAlignment.Center)
                afterList.Push(new XElement($"{{{NameSpace}}}{nameof(VerticalAlignment)}", $"{VerticalAlignment}"));
            while (afterList.Any())
                widthNode.AddAfterSelf(afterList.Pop());

            return result;
        }
    }

    public class GridCheckBoxColumnXmlDto : GridColumnXmlDto
    {
        public GridCheckBoxColumnXmlDto()
        {
            FilterRowCondition = "Equals";
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
        }

        public override XElement Serialize()
        {
            var result = base.Serialize();
            result.Add(new XAttribute("{http://www.w3.org/2001/XMLSchema-instance}type", "GridCheckBoxColumn"));

            var widthNode = result.Element($"{{{NameSpace}}}{nameof(Width)}");
            var afterList = new Stack<XElement>();
            if (HorizontalAlignment != HorizontalAlignment.Left)
                afterList.Push(new XElement($"{{{NameSpace}}}{nameof(HorizontalAlignment)}", $"{HorizontalAlignment}"));
            if (VerticalAlignment != VerticalAlignment.Top)
                afterList.Push(new XElement($"{{{NameSpace}}}{nameof(VerticalAlignment)}", $"{VerticalAlignment}"));
            while (afterList.Any())
                widthNode.AddAfterSelf(afterList.Pop());

            return result;
        }
    }

    public class GridTemplateColumnXmlDto : GridColumnXmlDto
    {
        public GridTemplateColumnXmlDto()
        {
            FilterRowCondition = "Equals";
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
        }

        public override XElement Serialize()
        {
            var result = base.Serialize();
            result.Add(new XAttribute("{http://www.w3.org/2001/XMLSchema-instance}type", "GridTemplateColumn"));

            var widthNode = result.Element($"{{{NameSpace}}}{nameof(Width)}");
            var afterList = new Stack<XElement>();
            if (HorizontalAlignment != HorizontalAlignment.Left)
                afterList.Push(new XElement($"{{{NameSpace}}}{nameof(HorizontalAlignment)}", $"{HorizontalAlignment}"));
            if (VerticalAlignment != VerticalAlignment.Top)
                afterList.Push(new XElement($"{{{NameSpace}}}{nameof(VerticalAlignment)}", $"{VerticalAlignment}"));
            while (afterList.Any())
                widthNode.AddAfterSelf(afterList.Pop());

            return result;
        }
    }

    public class SortColumnDescriptionXmlDto : IXmlDtoSerializable
    {
        public string ColumnName { get; set; } = null;

        public ListSortDirection SortDirection { get; set; } = ListSortDirection.Ascending;

        public XNamespace NameSpace =>
            "http://schemas.datacontract.org/2004/07/Syncfusion.UI.Xaml.Grid";

        public string LocalName =>
            "SortColumnDescription";

        public string XName =>
            $"{{{NameSpace}}}{LocalName}";

        public void Deserialize(XElement root)
        {
            if (root.Name != XName)
                return;
            ColumnName = root.ReadString($"{{{NameSpace}}}{nameof(ColumnName)}");
            SortDirection = root.ReadEnum<ListSortDirection>($"{{{NameSpace}}}{nameof(SortDirection)}") ?? ListSortDirection.Ascending;
        }

        public XElement Serialize()
        {
            var result = new XElement(XName);

            result.Add(new XElement($"{{{NameSpace}}}{nameof(ColumnName)}", ColumnName));
            if (SortDirection != ListSortDirection.Ascending)
                result.Add(new XElement($"{{{NameSpace}}}{nameof(SortDirection)}", $"{SortDirection}"));

            return result;
        }
    }

    public class GroupColumnDescriptionXmlDto
    { }

    public class StackedHeaderRowXmlDto
    { }

    public class GridSummaryRowXmlDto
    { }

    public class UnBoundRowXmlDto
    { }

    public class DetailsViewDefinitionXmlDto : IXmlDtoSerializable
    {
        public GridViewDefinitionXmlDto GridViewDefinition { get; set; }

        public XNamespace NameSpace =>
            "http://schemas.datacontract.org/2004/07/Syncfusion.UI.Xaml.Grid";

        public string LocalName =>
            nameof(DetailsViewDefinition);

        public string XName =>
            $"{{{NameSpace}}}{LocalName}";

        public void Deserialize(XElement root)
        {
            if (root.Name != XName)
                return;

            GridViewDefinition = root.ReadXmlSerializableObject<GridViewDefinitionXmlDto>($"{{{NameSpace}}}{nameof(GridViewDefinition)}");
        }

        public XElement Serialize()
        {
            var result = new XElement(XName);

            if (GridViewDefinition != null)
                result.Add(GridViewDefinition.Serialize());

            return result;
        }
    }

    public class GridViewDefinitionXmlDto : IXmlDtoSerializable
    {
        public DataGridXmlDto DataGrid { get; set; }

        public string RelationalColumn { get; set; }

        public XNamespace NameSpace =>
            "http://schemas.datacontract.org/2004/07/Syncfusion.UI.Xaml.Grid";

        public string LocalName =>
            nameof(GridViewDefinition);

        public string XName =>
            $"{{{NameSpace}}}{LocalName}";

        public void Deserialize(XElement root)
        {
            if (root.Name != XName)
                return;

            DataGrid = root.ReadXmlSerializableObject<DataGridXmlDto>($"{{{NameSpace}}}{nameof(DataGrid)}");
            RelationalColumn = root.ReadString($"{{{NameSpace}}}{nameof(RelationalColumn)}");
        }

        public XElement Serialize()
        {
            var result = new XElement(XName);

            if (DataGrid != null)
                result.Add(DataGrid.Serialize());
            if (!string.IsNullOrEmpty(RelationalColumn))
                result.Add(new XElement($"{{{NameSpace}}}{nameof(RelationalColumn)}", RelationalColumn));

            return result;
        }
    }
}
