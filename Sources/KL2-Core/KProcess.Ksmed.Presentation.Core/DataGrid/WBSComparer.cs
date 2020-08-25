using KProcess.Ksmed.Models;
using Syncfusion.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core
{
    public class WBSComparer : MarkupExtension, IComparer<object>, ISortDirection
    {
        public ListSortDirection SortDirection { get; set; }

        public int Compare(object x, object y)
        {
            if (x is PublishedAction xPublishedAction && y is PublishedAction yPublishedAction)
            {
                int[] x_SplittedWBS = string.IsNullOrEmpty(xPublishedAction.WBS) ? new int[] { } : xPublishedAction.WBS.Split('.').Select(str => int.Parse(str)).ToArray();
                int[] y_SplittedWBS = string.IsNullOrEmpty(yPublishedAction.WBS) ? new int[] { } : yPublishedAction.WBS.Split('.').Select(str => int.Parse(str)).ToArray();

                if (x_SplittedWBS.Length == 0 && y_SplittedWBS.Length == 0)
                    return 0;

                if (x_SplittedWBS.Length == 0)
                    return SortDirection == ListSortDirection.Ascending ? -1 : 1;

                if (y_SplittedWBS.Length == 0)
                    return SortDirection == ListSortDirection.Ascending ? 1 : -1;

                if (x_SplittedWBS.Length > y_SplittedWBS.Length)
                {
                    for (int i = 0; i < y_SplittedWBS.Length; i++)
                    {
                        if (x_SplittedWBS[i] != y_SplittedWBS[i])
                            return SortDirection == ListSortDirection.Ascending ?
                                (x_SplittedWBS[i] < y_SplittedWBS[i] ? -1 : 1) :
                                (x_SplittedWBS[i] < y_SplittedWBS[i] ? 1 : -1);
                    }
                    return SortDirection == ListSortDirection.Ascending ? 1 : -1;
                }

                if (x_SplittedWBS.Length < y_SplittedWBS.Length)
                {
                    for (int i = 0; i < x_SplittedWBS.Length; i++)
                    {
                        if (x_SplittedWBS[i] != y_SplittedWBS[i])
                            return SortDirection == ListSortDirection.Ascending ?
                                (x_SplittedWBS[i] < y_SplittedWBS[i] ? -1 : 1) :
                                (x_SplittedWBS[i] < y_SplittedWBS[i] ? 1 : -1);
                    }
                    return SortDirection == ListSortDirection.Ascending ? -1 : 1;
                }

                for (int i = 0; i < y_SplittedWBS.Length; i++)
                {
                    if (x_SplittedWBS[i] != y_SplittedWBS[i])
                        return SortDirection == ListSortDirection.Ascending ?
                            (x_SplittedWBS[i] < y_SplittedWBS[i] ? -1 : 1) :
                            (x_SplittedWBS[i] < y_SplittedWBS[i] ? 1 : -1);
                }
                return 0;
            }
            return 0;
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
