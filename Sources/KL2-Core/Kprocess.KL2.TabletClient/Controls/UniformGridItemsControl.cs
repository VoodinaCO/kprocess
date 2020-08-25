using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace Kprocess.KL2.TabletClient.Controls
{
    public class UniformGridItemsControl : ItemsControl
    {
        public UniformGridItemsControl()
        {
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(UniformGrid), nameof(UniformGrid));
            Binding bindingColumns = new Binding(nameof(UniformGridColumns))
            {
                Mode = BindingMode.OneWay,
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, GetType(), 1)
            };
            Binding bindingRows = new Binding(nameof(UniformGridRows))
            {
                Mode = BindingMode.OneWay,
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, GetType(), 1)
            };
            factory.SetBinding(UniformGrid.ColumnsProperty, bindingColumns);
            factory.SetBinding(UniformGrid.RowsProperty, bindingRows);

            ItemsPanel = new ItemsPanelTemplate(factory);
        }

        #region UniformGrid number of Columns and Rows control

        int UniformGridColumns
        {
            get { return (int)GetValue(UniformGridColumnsProperty); }
            set { SetValue(UniformGridColumnsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UniformGridColumns.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty UniformGridColumnsProperty =
            DependencyProperty.Register(nameof(UniformGridColumns), typeof(int), typeof(UniformGridItemsControl), new PropertyMetadata(4));

        private int UniformGridRows
        {
            get { return (int)GetValue(UniformGridRowsProperty); }
            set { SetValue(UniformGridRowsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UniformGridRows.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty UniformGridRowsProperty =
            DependencyProperty.Register(nameof(UniformGridRows), typeof(int), typeof(UniformGridItemsControl), new PropertyMetadata(4));

        public double MinColumnWidth
        {
            get { return (double)GetValue(MinColumnWidthProperty); }
            set { SetValue(MinColumnWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinColumnWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinColumnWidthProperty =
            DependencyProperty.Register(nameof(MinColumnWidth), typeof(double), typeof(UniformGridItemsControl),
                new PropertyMetadata(0d, OnDependencyPropertyChanged));

        public double MinRowHeight
        {
            get { return (double)GetValue(MinRowHeightProperty); }
            set { SetValue(MinRowHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinRowHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinRowHeightProperty =
            DependencyProperty.Register(nameof(MinRowHeight), typeof(double), typeof(UniformGridItemsControl),
                new PropertyMetadata(0d, OnDependencyPropertyChanged));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(UniformGridItemsControl),
                new PropertyMetadata(Orientation.Horizontal, OnDependencyPropertyChanged));

        private static void OnDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var itemsControl = (UniformGridItemsControl)d;
            itemsControl.OnSizeChanged();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            OnSizeChanged();
            base.OnRenderSizeChanged(sizeInfo);
        }

        private void OnSizeChanged()
        {
            if (ActualWidth == 0 || ActualHeight == 0) return;
            UniformGridColumns = (MinColumnWidth != 0) ? Math.Max((int)Math.Floor(ActualWidth / MinColumnWidth), 1) : 1;
            UniformGridRows = (MinRowHeight != 0) ? Math.Max((int)Math.Floor(ActualHeight / MinRowHeight), 1) : 1;
            OnDataChanged();
        }
        #endregion

        public IEnumerable MainItemsSource
        {
            get { return (IEnumerable)GetValue(MainItemsSourceProperty); }
            set { SetValue(MainItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MainItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MainItemsSourceProperty =
            DependencyProperty.Register(nameof(MainItemsSource), typeof(IEnumerable), typeof(UniformGridItemsControl),
                new PropertyMetadata(null, OnMainItemsSourceChanged));

        private static void OnMainItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var itemsControl = (UniformGridItemsControl)d;
            if (e.OldValue is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)e.OldValue).CollectionChanged -= itemsControl.UniformGridItemsControl_CollectionChanged;
            }
            if (e.NewValue is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)e.NewValue).CollectionChanged += itemsControl.UniformGridItemsControl_CollectionChanged;
            }

            itemsControl.OnDataChanged();
        }

        private void UniformGridItemsControl_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnDataChanged();
        }

        private int _pageIndex;
        private bool _isEnd;
        private RelayCommand _previousPageCommand;
        private RelayCommand _nextPageCommand;

        private void OnDataChanged()
        {
            if (MainItemsSource == null) return;
            var cells = UniformGridColumns * UniformGridRows;
            var enumerable = (MainItemsSource as IEnumerable<object>)?.ToArray();
            var itemsSource = enumerable.Skip(_pageIndex * cells).Take(cells);
            _isEnd = enumerable != null && enumerable.Count() <= cells * (_pageIndex + 1);
            _previousPageCommand?.RaiseCanExecuteChanged();
            _nextPageCommand?.RaiseCanExecuteChanged();

            if (Orientation == Orientation.Vertical)
            {
                var finished = false;
                var itemsSourceArray = itemsSource.ToArray();
                var reorderedItemsSource = new List<object>();
                for (var i = 0; i < UniformGridRows; i++)
                {
                    for (var j = 0; j < UniformGridColumns; j++)
                    {
                        var arrayIndex = i + j * UniformGridColumns;
                        reorderedItemsSource.Add((arrayIndex < itemsSource.Count()) ? itemsSourceArray[j + i] : null);
                        if (arrayIndex + 1 == itemsSource.Count())
                        {
                            finished = true;
                            break;
                        }
                    }
                    if (finished) break;
                }
                ItemsSource = reorderedItemsSource;
            }
            else
            {
                ItemsSource = itemsSource;
            }
        }

        public ICommand PreviousPageCommand => _previousPageCommand ?? (_previousPageCommand = new RelayCommand(() =>
        {
            _pageIndex--;
            OnDataChanged();
        }, () => _pageIndex > 0));

        public ICommand NextPageCommand => _nextPageCommand ?? (_nextPageCommand = new RelayCommand(() =>
        {
            _pageIndex++;
            OnDataChanged();
        }, () => !_isEnd));

    }
}
