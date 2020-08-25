using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security.Extensions;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    public class UserCanWriteComparer : DependencyObject, IComparer<object>, ISortDirection
    {
        public static readonly DependencyProperty CurrentProcessProperty =
            DependencyProperty.Register(nameof(CurrentProcess), typeof(Project), typeof(UserCanWriteComparer));
        public Procedure CurrentProcess
        {
            get => (Procedure)GetValue(CurrentProcessProperty);
            set => SetValue(CurrentProcessProperty, value);
        }

        public static readonly DependencyProperty SfDataGridProperty =
            DependencyProperty.Register(nameof(SfDataGrid), typeof(SfDataGrid), typeof(UserCanWriteComparer));
        public SfDataGrid SfDataGrid
        {
            get => (SfDataGrid)GetValue(SfDataGridProperty);
            set => SetValue(SfDataGridProperty, value);
        }

        public ListSortDirection SortDirection
        {
            get => SfDataGrid.SortColumnDescriptions.First().SortDirection;
            set
            {
                return;
            }
        }

        public int Compare(object x, object y)
        {
            if (x is User user1 && y is User user2)
            {
                if (SortDirection == ListSortDirection.Ascending)
                {
                    if (CurrentProcess.CanWrite(user1) == CurrentProcess.CanWrite(user2))
                        return string.Compare(user1.FullName, user2.FullName);
                    if (CurrentProcess.CanWrite(user1) && !CurrentProcess.CanWrite(user2))
                        return 1;
                    if (!CurrentProcess.CanWrite(user1) && CurrentProcess.CanWrite(user2))
                        return -1;
                }
                else
                {
                    if (CurrentProcess.CanWrite(user1) == CurrentProcess.CanWrite(user2))
                        return -string.Compare(user1.FullName, user2.FullName);
                    if (CurrentProcess.CanWrite(user1) && !CurrentProcess.CanWrite(user2))
                        return 1;
                    if (!CurrentProcess.CanWrite(user1) && CurrentProcess.CanWrite(user2))
                        return -1;
                }
            }
            return 0;
        }
    }

    public class UserCanReadComparer : DependencyObject, IComparer<object>, ISortDirection
    {
        readonly UserCanWriteComparer canWriteComparer = new UserCanWriteComparer();

        public static readonly DependencyProperty CurrentProcessProperty =
            DependencyProperty.Register(nameof(CurrentProcess), typeof(Project), typeof(UserCanReadComparer));
        public Procedure CurrentProcess
        {
            get => (Procedure)GetValue(CurrentProcessProperty);
            set => SetValue(CurrentProcessProperty, value);
        }

        public static readonly DependencyProperty SfDataGridProperty =
            DependencyProperty.Register(nameof(SfDataGrid), typeof(SfDataGrid), typeof(UserCanReadComparer));
        public SfDataGrid SfDataGrid
        {
            get => (SfDataGrid)GetValue(SfDataGridProperty);
            set => SetValue(SfDataGridProperty, value);
        }

        public ListSortDirection SortDirection
        {
            get => SfDataGrid.SortColumnDescriptions.First().SortDirection;
            set
            {
                return;
            }
        }

        public int Compare(object x, object y)
        {
            canWriteComparer.CurrentProcess = CurrentProcess;
            canWriteComparer.SfDataGrid = SfDataGrid;
            if (x is User user1 && y is User user2)
            {
                if (SortDirection == ListSortDirection.Ascending)
                {
                    if (CurrentProcess.CanRead(user1) == CurrentProcess.CanRead(user2))
                        return canWriteComparer.Compare(user1, user2);
                    if (CurrentProcess.CanRead(user1) && !CurrentProcess.CanRead(user2))
                        return 1;
                    if (!CurrentProcess.CanRead(user1) && CurrentProcess.CanRead(user2))
                        return -1;
                }
                else
                {
                    if (CurrentProcess.CanRead(user1) == CurrentProcess.CanRead(user2))
                        return canWriteComparer.Compare(user1, user2);
                    if (CurrentProcess.CanRead(user1) && !CurrentProcess.CanRead(user2))
                        return 1;
                    if (!CurrentProcess.CanRead(user1) && CurrentProcess.CanRead(user2))
                        return -1;
                }
            }
            return 0;
        }
    }
}
