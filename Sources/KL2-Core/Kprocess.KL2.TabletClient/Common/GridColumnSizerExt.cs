using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kprocess.KL2.TabletClient.Common
{
    public class GridColumnSizerExt : GridColumnSizer
    {
        public GridColumnSizerExt(SfDataGrid grid)
            : base(grid)
        {
        }

        protected override void SetStarWidth(double remainingColumnWidth, IEnumerable<GridColumn> remainingColumns)
        {
            List<GridColumn> removedColumn = new List<GridColumn>();
            List<GridColumn> columns = remainingColumns.ToList();
            double totalRemainingStarValue = remainingColumnWidth;
            double removedWidth = 0;
            bool isRemoved;

            while (columns.Count > 0)
            {
                isRemoved = false;
                removedWidth = 0;
                int columnsCount = 0;

                columns.ForEach((col) =>
                {
                    columnsCount += StarRatio.GetColumnRatio(col);
                });

                double starWidth = Math.Floor((totalRemainingStarValue / columnsCount));
                GridColumn column = columns.First();
                starWidth *= StarRatio.GetColumnRatio(column);
                double computedWidth = SetColumnWidth(column, starWidth);

                if (starWidth != computedWidth && starWidth > 0)
                {
                    isRemoved = true;
                    columns.Remove(column);

                    foreach (GridColumn remColumn in removedColumn)
                    {
                        if (!columns.Contains(remColumn))
                        {
                            removedWidth += remColumn.ActualWidth;
                            columns.Add(remColumn);
                        }
                    }
                    removedColumn.Clear();
                    totalRemainingStarValue += removedWidth;
                }

                totalRemainingStarValue = totalRemainingStarValue - computedWidth;

                if (!isRemoved)
                {
                    columns.Remove(column);

                    if (!removedColumn.Contains(column))
                        removedColumn.Add(column);
                }
            }
        }
    }
}
