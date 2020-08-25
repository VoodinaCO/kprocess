

namespace KProcess.KL2.WebAdmin.Models.DTO
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.datacontract.org/2004/07/Syncfusion.UI.Xaml.Grid")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.datacontract.org/2004/07/Syncfusion.UI.Xaml.Grid", IsNullable = false)]
    public class SfDataGrid
    {
        private bool allowDraggingColumnsField;

        private bool allowGroupingField;

        private bool allowResizingColumnsField;

        private bool allowSortField;

        private string columnSizerField;

        private SfDataGridGridColumn[] columnsField;

        private SfDataGridCurrentCellBorderThickness currentCellBorderThicknessField;

        private byte dataFetchSizeField;

        private object detailsViewDefinitionField;

        private SfDataGridDetailsViewPadding detailsViewPaddingField;

        private string editTriggerField;

        private byte expanderColumnWidthField;

        private object filterSettingsField;

        private string gridCopyOptionField;

        private string gridPasteOptionField;

        private string gridValidationModeField;

        private object groupColumnDescriptionsField;

        private string groupDropAreaTextField;

        private object groupSummaryRowsField;

        private byte headerRowHeightField;

        private byte indentColumnWidthField;

        private string liveDataUpdateModeField;

        private bool reuseRowsOnItemssourceChangeField;

        private byte rowHeaderWidthField;

        private byte rowHeightField;

        private string selectionModeField;

        private bool showColumnWhenGroupedField;

        private SfDataGridSortColumnDescriptions sortColumnDescriptionsField;

        private object stackedHeaderRowsField;

        private object tableSummaryRowsField;

        private object unBoundRowsField;

        /// <remarks/>
        public bool AllowDraggingColumns
        {
            get
            {
                return this.allowDraggingColumnsField;
            }
            set
            {
                this.allowDraggingColumnsField = value;
            }
        }

        /// <remarks/>
        public bool AllowGrouping
        {
            get
            {
                return this.allowGroupingField;
            }
            set
            {
                this.allowGroupingField = value;
            }
        }

        /// <remarks/>
        public bool AllowResizingColumns
        {
            get
            {
                return this.allowResizingColumnsField;
            }
            set
            {
                this.allowResizingColumnsField = value;
            }
        }

        /// <remarks/>
        public bool AllowSort
        {
            get
            {
                return this.allowSortField;
            }
            set
            {
                this.allowSortField = value;
            }
        }

        /// <remarks/>
        public string ColumnSizer
        {
            get
            {
                return this.columnSizerField;
            }
            set
            {
                this.columnSizerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("GridColumn", IsNullable = false)]
        public SfDataGridGridColumn[] Columns
        {
            get
            {
                return this.columnsField;
            }
            set
            {
                this.columnsField = value;
            }
        }

        /// <remarks/>
        public SfDataGridCurrentCellBorderThickness CurrentCellBorderThickness
        {
            get
            {
                return this.currentCellBorderThicknessField;
            }
            set
            {
                this.currentCellBorderThicknessField = value;
            }
        }

        /// <remarks/>
        public byte DataFetchSize
        {
            get
            {
                return this.dataFetchSizeField;
            }
            set
            {
                this.dataFetchSizeField = value;
            }
        }

        /// <remarks/>
        public object DetailsViewDefinition
        {
            get
            {
                return this.detailsViewDefinitionField;
            }
            set
            {
                this.detailsViewDefinitionField = value;
            }
        }

        /// <remarks/>
        public SfDataGridDetailsViewPadding DetailsViewPadding
        {
            get
            {
                return this.detailsViewPaddingField;
            }
            set
            {
                this.detailsViewPaddingField = value;
            }
        }

        /// <remarks/>
        public string EditTrigger
        {
            get
            {
                return this.editTriggerField;
            }
            set
            {
                this.editTriggerField = value;
            }
        }

        /// <remarks/>
        public byte ExpanderColumnWidth
        {
            get
            {
                return this.expanderColumnWidthField;
            }
            set
            {
                this.expanderColumnWidthField = value;
            }
        }

        /// <remarks/>
        public object FilterSettings
        {
            get
            {
                return this.filterSettingsField;
            }
            set
            {
                this.filterSettingsField = value;
            }
        }

        /// <remarks/>
        public string GridCopyOption
        {
            get
            {
                return this.gridCopyOptionField;
            }
            set
            {
                this.gridCopyOptionField = value;
            }
        }

        /// <remarks/>
        public string GridPasteOption
        {
            get
            {
                return this.gridPasteOptionField;
            }
            set
            {
                this.gridPasteOptionField = value;
            }
        }

        /// <remarks/>
        public string GridValidationMode
        {
            get
            {
                return this.gridValidationModeField;
            }
            set
            {
                this.gridValidationModeField = value;
            }
        }

        /// <remarks/>
        public object GroupColumnDescriptions
        {
            get
            {
                return this.groupColumnDescriptionsField;
            }
            set
            {
                this.groupColumnDescriptionsField = value;
            }
        }

        /// <remarks/>
        public string GroupDropAreaText
        {
            get
            {
                return this.groupDropAreaTextField;
            }
            set
            {
                this.groupDropAreaTextField = value;
            }
        }

        /// <remarks/>
        public object GroupSummaryRows
        {
            get
            {
                return this.groupSummaryRowsField;
            }
            set
            {
                this.groupSummaryRowsField = value;
            }
        }

        /// <remarks/>
        public byte HeaderRowHeight
        {
            get
            {
                return this.headerRowHeightField;
            }
            set
            {
                this.headerRowHeightField = value;
            }
        }

        /// <remarks/>
        public byte IndentColumnWidth
        {
            get
            {
                return this.indentColumnWidthField;
            }
            set
            {
                this.indentColumnWidthField = value;
            }
        }

        /// <remarks/>
        public string LiveDataUpdateMode
        {
            get
            {
                return this.liveDataUpdateModeField;
            }
            set
            {
                this.liveDataUpdateModeField = value;
            }
        }

        /// <remarks/>
        public bool ReuseRowsOnItemssourceChange
        {
            get
            {
                return this.reuseRowsOnItemssourceChangeField;
            }
            set
            {
                this.reuseRowsOnItemssourceChangeField = value;
            }
        }

        /// <remarks/>
        public byte RowHeaderWidth
        {
            get
            {
                return this.rowHeaderWidthField;
            }
            set
            {
                this.rowHeaderWidthField = value;
            }
        }

        /// <remarks/>
        public byte RowHeight
        {
            get
            {
                return this.rowHeightField;
            }
            set
            {
                this.rowHeightField = value;
            }
        }

        /// <remarks/>
        public string SelectionMode
        {
            get
            {
                return this.selectionModeField;
            }
            set
            {
                this.selectionModeField = value;
            }
        }

        /// <remarks/>
        public bool ShowColumnWhenGrouped
        {
            get
            {
                return this.showColumnWhenGroupedField;
            }
            set
            {
                this.showColumnWhenGroupedField = value;
            }
        }

        /// <remarks/>
        public SfDataGridSortColumnDescriptions SortColumnDescriptions
        {
            get
            {
                return this.sortColumnDescriptionsField;
            }
            set
            {
                this.sortColumnDescriptionsField = value;
            }
        }

        /// <remarks/>
        public object StackedHeaderRows
        {
            get
            {
                return this.stackedHeaderRowsField;
            }
            set
            {
                this.stackedHeaderRowsField = value;
            }
        }

        /// <remarks/>
        public object TableSummaryRows
        {
            get
            {
                return this.tableSummaryRowsField;
            }
            set
            {
                this.tableSummaryRowsField = value;
            }
        }

        /// <remarks/>
        public object UnBoundRows
        {
            get
            {
                return this.unBoundRowsField;
            }
            set
            {
                this.unBoundRowsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.datacontract.org/2004/07/Syncfusion.UI.Xaml.Grid")]
    public partial class SfDataGridGridColumn
    {

        private bool allowBlankFiltersField;

        private bool allowEditingField;

        private bool allowEditingFieldSpecified;

        private bool allowFilteringField;

        private bool allowFilteringFieldSpecified;

        private bool allowFocusField;

        private bool allowGroupingField;

        private bool allowGroupingFieldSpecified;

        private bool allowResizingField;

        private bool allowResizingFieldSpecified;

        private bool allowSortingField;

        private bool allowSortingFieldSpecified;

        private string columnSizerField;

        private string filterRowConditionField;

        private string filterRowEditorTypeField;

        private string filteredFromField;

        private string gridValidationModeField;

        private string headerTextField;

        private string horizontalHeaderContentAlignmentField;

        private bool isHiddenField;

        private bool isHiddenFieldSpecified;

        private string mappingNameField;

        private float maximumWidthField;

        private float minimumWidthField;

        private string textAlignmentField;

        private float widthField;

        private string horizontalAlignmentField;

        private string verticalAlignmentField;

        private string maxValueField;

        private string minValueField;

        private object nullTextField;

        private byte numberDecimalDigitsField;

        private bool numberDecimalDigitsFieldSpecified;

        private string numberDecimalSeparatorField;

        private string numberGroupSeparatorField;

        private SfDataGridGridColumnNumberGroupSizes numberGroupSizesField;

        private byte numberNegativePatternField;

        private bool numberNegativePatternFieldSpecified;

        private string textWrappingField;

        /// <remarks/>
        public bool AllowBlankFilters
        {
            get
            {
                return this.allowBlankFiltersField;
            }
            set
            {
                this.allowBlankFiltersField = value;
            }
        }

        /// <remarks/>
        public bool AllowEditing
        {
            get
            {
                return this.allowEditingField;
            }
            set
            {
                this.allowEditingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AllowEditingSpecified
        {
            get
            {
                return this.allowEditingFieldSpecified;
            }
            set
            {
                this.allowEditingFieldSpecified = value;
            }
        }

        /// <remarks/>
        public bool AllowFiltering
        {
            get
            {
                return this.allowFilteringField;
            }
            set
            {
                this.allowFilteringField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AllowFilteringSpecified
        {
            get
            {
                return this.allowFilteringFieldSpecified;
            }
            set
            {
                this.allowFilteringFieldSpecified = value;
            }
        }

        /// <remarks/>
        public bool AllowFocus
        {
            get
            {
                return this.allowFocusField;
            }
            set
            {
                this.allowFocusField = value;
            }
        }

        /// <remarks/>
        public bool AllowGrouping
        {
            get
            {
                return this.allowGroupingField;
            }
            set
            {
                this.allowGroupingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AllowGroupingSpecified
        {
            get
            {
                return this.allowGroupingFieldSpecified;
            }
            set
            {
                this.allowGroupingFieldSpecified = value;
            }
        }

        /// <remarks/>
        public bool AllowResizing
        {
            get
            {
                return this.allowResizingField;
            }
            set
            {
                this.allowResizingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AllowResizingSpecified
        {
            get
            {
                return this.allowResizingFieldSpecified;
            }
            set
            {
                this.allowResizingFieldSpecified = value;
            }
        }

        /// <remarks/>
        public bool AllowSorting
        {
            get
            {
                return this.allowSortingField;
            }
            set
            {
                this.allowSortingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AllowSortingSpecified
        {
            get
            {
                return this.allowSortingFieldSpecified;
            }
            set
            {
                this.allowSortingFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string ColumnSizer
        {
            get
            {
                return this.columnSizerField;
            }
            set
            {
                this.columnSizerField = value;
            }
        }

        /// <remarks/>
        public string FilterRowCondition
        {
            get
            {
                return this.filterRowConditionField;
            }
            set
            {
                this.filterRowConditionField = value;
            }
        }

        /// <remarks/>
        public string FilterRowEditorType
        {
            get
            {
                return this.filterRowEditorTypeField;
            }
            set
            {
                this.filterRowEditorTypeField = value;
            }
        }

        /// <remarks/>
        public string FilteredFrom
        {
            get
            {
                return this.filteredFromField;
            }
            set
            {
                this.filteredFromField = value;
            }
        }

        /// <remarks/>
        public string GridValidationMode
        {
            get
            {
                return this.gridValidationModeField;
            }
            set
            {
                this.gridValidationModeField = value;
            }
        }

        /// <remarks/>
        public string HeaderText
        {
            get
            {
                return this.headerTextField;
            }
            set
            {
                this.headerTextField = value;
            }
        }

        /// <remarks/>
        public string HorizontalHeaderContentAlignment
        {
            get
            {
                return this.horizontalHeaderContentAlignmentField;
            }
            set
            {
                this.horizontalHeaderContentAlignmentField = value;
            }
        }

        /// <remarks/>
        public bool IsHidden
        {
            get
            {
                return this.isHiddenField;
            }
            set
            {
                this.isHiddenField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsHiddenSpecified
        {
            get
            {
                return this.isHiddenFieldSpecified;
            }
            set
            {
                this.isHiddenFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string MappingName
        {
            get
            {
                return this.mappingNameField;
            }
            set
            {
                this.mappingNameField = value;
            }
        }

        /// <remarks/>
        public float MaximumWidth
        {
            get
            {
                return this.maximumWidthField;
            }
            set
            {
                this.maximumWidthField = value;
            }
        }

        /// <remarks/>
        public float MinimumWidth
        {
            get
            {
                return this.minimumWidthField;
            }
            set
            {
                this.minimumWidthField = value;
            }
        }

        /// <remarks/>
        public string TextAlignment
        {
            get
            {
                return this.textAlignmentField;
            }
            set
            {
                this.textAlignmentField = value;
            }
        }

        /// <remarks/>
        public float Width
        {
            get
            {
                return this.widthField;
            }
            set
            {
                this.widthField = value;
            }
        }

        /// <remarks/>
        public string HorizontalAlignment
        {
            get
            {
                return this.horizontalAlignmentField;
            }
            set
            {
                this.horizontalAlignmentField = value;
            }
        }

        /// <remarks/>
        public string VerticalAlignment
        {
            get
            {
                return this.verticalAlignmentField;
            }
            set
            {
                this.verticalAlignmentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "integer")]
        public string MaxValue
        {
            get
            {
                return this.maxValueField;
            }
            set
            {
                this.maxValueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "integer")]
        public string MinValue
        {
            get
            {
                return this.minValueField;
            }
            set
            {
                this.minValueField = value;
            }
        }

        /// <remarks/>
        public object NullText
        {
            get
            {
                return this.nullTextField;
            }
            set
            {
                this.nullTextField = value;
            }
        }

        /// <remarks/>
        public byte NumberDecimalDigits
        {
            get
            {
                return this.numberDecimalDigitsField;
            }
            set
            {
                this.numberDecimalDigitsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool NumberDecimalDigitsSpecified
        {
            get
            {
                return this.numberDecimalDigitsFieldSpecified;
            }
            set
            {
                this.numberDecimalDigitsFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string NumberDecimalSeparator
        {
            get
            {
                return this.numberDecimalSeparatorField;
            }
            set
            {
                this.numberDecimalSeparatorField = value;
            }
        }

        /// <remarks/>
        public string NumberGroupSeparator
        {
            get
            {
                return this.numberGroupSeparatorField;
            }
            set
            {
                this.numberGroupSeparatorField = value;
            }
        }

        /// <remarks/>
        public SfDataGridGridColumnNumberGroupSizes NumberGroupSizes
        {
            get
            {
                return this.numberGroupSizesField;
            }
            set
            {
                this.numberGroupSizesField = value;
            }
        }

        /// <remarks/>
        public byte NumberNegativePattern
        {
            get
            {
                return this.numberNegativePatternField;
            }
            set
            {
                this.numberNegativePatternField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool NumberNegativePatternSpecified
        {
            get
            {
                return this.numberNegativePatternFieldSpecified;
            }
            set
            {
                this.numberNegativePatternFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string TextWrapping
        {
            get
            {
                return this.textWrappingField;
            }
            set
            {
                this.textWrappingField = value;
            }
        }
    }



    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.datacontract.org/2004/07/Syncfusion.UI.Xaml.Grid")]
    public partial class SfDataGridGridColumnNumberGroupSizes
    {

        private byte intField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays")]
        public byte @int
        {
            get
            {
                return this.intField;
            }
            set
            {
                this.intField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.datacontract.org/2004/07/Syncfusion.UI.Xaml.Grid")]
    public partial class SfDataGridCurrentCellBorderThickness
    {

        private byte bottomField;

        private byte leftField;

        private byte rightField;

        private byte topField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://schemas.datacontract.org/2004/07/System.Windows")]
        public byte Bottom
        {
            get
            {
                return this.bottomField;
            }
            set
            {
                this.bottomField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://schemas.datacontract.org/2004/07/System.Windows")]
        public byte Left
        {
            get
            {
                return this.leftField;
            }
            set
            {
                this.leftField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://schemas.datacontract.org/2004/07/System.Windows")]
        public byte Right
        {
            get
            {
                return this.rightField;
            }
            set
            {
                this.rightField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://schemas.datacontract.org/2004/07/System.Windows")]
        public byte Top
        {
            get
            {
                return this.topField;
            }
            set
            {
                this.topField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.datacontract.org/2004/07/Syncfusion.UI.Xaml.Grid")]
    public partial class SfDataGridDetailsViewPadding
    {

        private byte bottomField;

        private byte leftField;

        private byte rightField;

        private byte topField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://schemas.datacontract.org/2004/07/System.Windows")]
        public byte Bottom
        {
            get
            {
                return this.bottomField;
            }
            set
            {
                this.bottomField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://schemas.datacontract.org/2004/07/System.Windows")]
        public byte Left
        {
            get
            {
                return this.leftField;
            }
            set
            {
                this.leftField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://schemas.datacontract.org/2004/07/System.Windows")]
        public byte Right
        {
            get
            {
                return this.rightField;
            }
            set
            {
                this.rightField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://schemas.datacontract.org/2004/07/System.Windows")]
        public byte Top
        {
            get
            {
                return this.topField;
            }
            set
            {
                this.topField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.datacontract.org/2004/07/Syncfusion.UI.Xaml.Grid")]
    public partial class SfDataGridSortColumnDescriptions
    {

        private SfDataGridSortColumnDescriptionsSortColumnDescription sortColumnDescriptionField;

        /// <remarks/>
        public SfDataGridSortColumnDescriptionsSortColumnDescription SortColumnDescription
        {
            get
            {
                return this.sortColumnDescriptionField;
            }
            set
            {
                this.sortColumnDescriptionField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.datacontract.org/2004/07/Syncfusion.UI.Xaml.Grid")]
    public partial class SfDataGridSortColumnDescriptionsSortColumnDescription
    {

        private string columnNameField;

        /// <remarks/>
        public string ColumnName
        {
            get
            {
                return this.columnNameField;
            }
            set
            {
                this.columnNameField = value;
            }
        }
    }

}