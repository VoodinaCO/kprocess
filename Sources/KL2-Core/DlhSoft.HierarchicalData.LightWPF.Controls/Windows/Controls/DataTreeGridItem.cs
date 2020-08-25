using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace DlhSoft.Windows.Controls
{
    public class DataTreeGridItem : DependencyObject, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(DataTreeGridItem), new PropertyMetadata(null, new PropertyChangedCallback(OnContentChanged)));
        public static readonly DependencyProperty ExpanderVisibilityProperty = DependencyProperty.Register(nameof(ExpanderVisibility), typeof(Visibility), typeof(DataTreeGridItem), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty HasChildrenProperty = DependencyProperty.Register(nameof(HasChildren), typeof(bool), typeof(DataTreeGridItem), new PropertyMetadata(false, new PropertyChangedCallback(OnHasChildrenChanged)));
        public static readonly DependencyProperty IndentationProperty = DependencyProperty.Register(nameof(Indentation), typeof(int), typeof(DataTreeGridItem), new PropertyMetadata(0, new PropertyChangedCallback(OnIndentationChanged)));
        public static readonly DependencyProperty IndentationWidthProperty = DependencyProperty.Register(nameof(IndentationWidth), typeof(double), typeof(DataTreeGridItem), new PropertyMetadata(0.0));
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(DataTreeGridItem), new PropertyMetadata(true, new PropertyChangedCallback(OnIsExpandedChanged)));
        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register(nameof(IsVisible), typeof(bool), typeof(DataTreeGridItem), new PropertyMetadata(true, new PropertyChangedCallback(OnIsVisibleChanged)));
        public static readonly DependencyProperty TagProperty = DependencyProperty.Register(nameof(Tag), typeof(object), typeof(DataTreeGridItem), new PropertyMetadata(null));
        public static readonly DependencyProperty VisibilityProperty = DependencyProperty.Register(nameof(Visibility), typeof(Visibility), typeof(DataTreeGridItem), new PropertyMetadata(Visibility.Visible));

        public event EventHandler ContentChanged;

        public event EventHandler ExpansionChanged;

        public event EventHandler HasChildrenChanged;

        public event EventHandler HierarchyChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler VisibilityChanged;

        protected virtual void OnContentChanged() =>
            ContentChanged?.Invoke(this, EventArgs.Empty);

        static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataTreeGridItem item)
            {
                item.OnPropertyChanged(nameof(Content));
                item.OnContentChanged();
            }
        }

        protected virtual void OnExpansionChanged() =>
            ExpansionChanged?.Invoke(this, EventArgs.Empty);

        protected virtual void OnHasChildrenChanged() =>
            HasChildrenChanged?.Invoke(this, EventArgs.Empty);

        static void OnHasChildrenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataTreeGridItem item)
            {
                item.OnPropertyChanged(nameof(HasChildren));
                item.OnHasChildrenChanged();
                item.ExpanderVisibility = item.HasChildren ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        protected virtual void OnIndentationChanged() =>
            HierarchyChanged?.Invoke(this, EventArgs.Empty);

        static void OnIndentationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataTreeGridItem item)
            {
                item.OnPropertyChanged(nameof(Indentation));
                item.OnIndentationChanged();
                item.UpdateIndentationWidth();
            }
        }

        static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataTreeGridItem item)
            {
                item.OnPropertyChanged(nameof(IsExpanded));
                item.OnExpansionChanged();
            }
        }

        static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataTreeGridItem item)
            {
                item.OnPropertyChanged(nameof(IsVisible));
                item.OnVisibilityChanged();
                item.Visibility = item.IsVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected virtual void OnVisibilityChanged() =>
            VisibilityChanged?.Invoke(this, EventArgs.Empty);

        public override string ToString() =>
            Content?.ToString() ?? string.Empty;

        internal void UpdateIndentationWidth()
        {
            if (DataTreeGrid != null)
                IndentationWidth = Indentation * DataTreeGrid.IndentationUnitSize;
        }

        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public DataTreeGrid DataTreeGrid { get; internal set; }

        public Visibility ExpanderVisibility
        {
            get => (Visibility)GetValue(ExpanderVisibilityProperty);
            set => SetValue(ExpanderVisibilityProperty, value);
        }

        public bool HasChildren
        {
            get => (bool)GetValue(HasChildrenProperty);
            set => SetValue(HasChildrenProperty, value);
        }

        public int Indentation
        {
            get => (int)GetValue(IndentationProperty);
            set => SetValue(IndentationProperty, value);
        }

        public double IndentationWidth
        {
            get => (double)GetValue(IndentationWidthProperty);
            set => SetValue(IndentationWidthProperty, value);
        }

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public bool IsVisible
        {
            get => (bool)GetValue(IsVisibleProperty);
            set => SetValue(IsVisibleProperty, value);
        }

        public object Tag
        {
            get => GetValue(TagProperty);
            set => SetValue(TagProperty, value);
        }

        public Visibility Visibility
        {
            get => (Visibility)GetValue(VisibilityProperty);
            set => SetValue(VisibilityProperty, value);
        }
    }
}

