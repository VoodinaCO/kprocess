using Syncfusion.UI.Xaml.Grid;
using System.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    public class LinkedPublicationDataGridSerialization : SerializationController
    {
        public LinkedPublicationDataGridSerialization(SfDataGrid dataGrid)
            : base(dataGrid)
        {
        }

        protected override void RestoreColumnProperties(SerializableGridColumn serializableColumn, GridColumn column)
        {
            base.RestoreColumnProperties(serializableColumn, column);

            if (column.MappingName.StartsWith("Refs") || column.MappingName.StartsWith("CustomNumericValue") || column.MappingName.StartsWith("CustomTextValue"))
                column.HeaderTemplate = Application.Current.Resources[$"{column.MappingName}HeaderLinkedTemplate"] as DataTemplate;

            if (column.MappingName.EndsWith("Start") || column.MappingName.EndsWith("Finish"))
                column.CellTemplate = Application.Current.Resources[$"{column.MappingName}Template"] as DataTemplate;

            if (column is GridTemplateColumn && Application.Current.Resources.Contains($"{column.MappingName}Template"))
                column.CellTemplate = Application.Current.Resources[$"{column.MappingName}Template"] as DataTemplate;
        }
    }
}
