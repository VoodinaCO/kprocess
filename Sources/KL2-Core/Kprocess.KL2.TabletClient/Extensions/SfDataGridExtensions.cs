using Kprocess.KL2.TabletClient.Globalization;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kprocess.KL2.TabletClient.Extensions
{
    public static class SfDataGridExtensions
    {
        public static void RemoveHiddenColumns(this SfDataGrid dataGrid)
        {
            List<GridColumn> columnsToDelete = dataGrid.Columns.Where(_ => _.IsHidden).ToList();
            columnsToDelete.ForEach(_ => dataGrid.Columns.Remove(_));
        }

        public static void RenameColumns(this SfDataGrid dataGrid)
        {
            dataGrid.Columns.ForEach(_ =>
            {
                if (_.MappingName == "Label")
                    _.HeaderText = Locator.LocalizationManager.GetString("Grid_LabelHeaderText");
                else if (_.MappingName == "Thumbnail")
                    _.HeaderText = Locator.LocalizationManager.GetString("Grid_ThumbnailHeaderText");
            });
        }

        public static void CustomDeserialize(this SfDataGrid dataGrid, byte[] disposition, bool deserializeDetailsViewDefinition = false)
        {
            DeserializationOptions options = new DeserializationOptions
            {
                DeserializeColumns = true,
                DeserializeSorting = true
            };
            if (deserializeDetailsViewDefinition)
                options.DeserializeDetailsViewDefinition = true;

            using (MemoryStream ms = new MemoryStream(disposition))
            {
                dataGrid.Deserialize(ms, options);
                dataGrid.AllowDraggingColumns = false;
                dataGrid.AllowResizingColumns = false;
                dataGrid.AllowResizingHiddenColumns = false;
                dataGrid.AllowSorting = false;
                dataGrid.AllowEditing = false;
                dataGrid.HideEmptyGridViewDefinition = true;
            }
        }
    }
}
