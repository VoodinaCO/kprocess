using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DlhSoft.Windows.Controls
{
    /// <summary>
    /// Interaction logic for TaskPresenter.xaml
    /// </summary>
    public partial class TaskPresenter : UserControl
    {
        public static readonly DependencyProperty IsVirtuallyVisibleProperty = DependencyProperty.Register("IsVirtuallyVisible", typeof(bool), typeof(TaskPresenter), new PropertyMetadata(false, new PropertyChangedCallback(TaskPresenter.OnIsVirtuallyVisibleChanged)));

        /// <summary>
        /// Obtient ou définit .
        /// </summary>
        public Visibility DependencyLinesVisibility
        {
            get { return (Visibility)GetValue(DependencyLinesVisibilityProperty); }
            set { SetValue(DependencyLinesVisibilityProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="DependencyLinesVisibility"/>.
        /// </summary>
        public static readonly DependencyProperty DependencyLinesVisibilityProperty = GanttChartDataGrid.DependencyLinesVisibilityProperty.AddOwner(typeof(TaskPresenter), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Obtient ou définit la visibilité du Thumb de création de dépendance sur le Chart.
        /// </summary>
        public Visibility ChartDependencyCreationThumbVisibility
        {
            get { return (Visibility)GetValue(ChartDependencyCreationThumbVisibilityProperty); }
            set { SetValue(ChartDependencyCreationThumbVisibilityProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="ChartDependencyCreationThumbVisibility"/>.
        /// </summary>
        public static readonly DependencyProperty ChartDependencyCreationThumbVisibilityProperty =
            DependencyProperty.Register("ChartDependencyCreationThumbVisibility", typeof(Visibility), typeof(TaskPresenter),
            new UIPropertyMetadata(Visibility.Visible, OnChartDependencyCreationThumbVisibilityChanged));


        /// <summary>
        /// Obtient ou définit la visibilité du Thumb de création de dépendance.
        /// </summary>
        public Visibility DependencyCreationThumbVisibility
        {
            get { return (Visibility)GetValue(DependencyCreationThumbVisibilityProperty); }
            set { SetValue(DependencyCreationThumbVisibilityProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="DependencyCreationThumbVisibility"/>.
        /// </summary>
        public static readonly DependencyProperty DependencyCreationThumbVisibilityProperty =
            DependencyProperty.Register("DependencyCreationThumbVisibility", typeof(Visibility), typeof(TaskPresenter), new PropertyMetadata(Visibility.Visible, OnChartDependencyCreationThumbVisibilityChanged));

        /// <summary>
        /// Appelé lorsque la valeur de la propriété de dépendance <see cref="ChartDependencyCreationThumbVisibility"/> a changé.
        /// </summary>
        /// <param name="d">L'instance où la propriété a changé.</param>
        /// <param name="e">Les arguments de l'évènement <see cref="System.Windows.DependencyPropertyChangedEventArgs"/>.</param>
        private static void OnChartDependencyCreationThumbVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (TaskPresenter)d;
            bool visible = source.ChartDependencyCreationThumbVisibility == Visibility.Visible && source.DependencyCreationThumbVisibility == Visibility.Visible;

            source.DragDependencyThumb.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public TaskPresenter()
        {
            InitializeComponent();
        }

        private static void OnIsVirtuallyVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskPresenter presenter = d as TaskPresenter;
            if (presenter != null)
            {
                presenter.UpdateIsVirtuallyVisible();
            }
        }

        internal void UpdateIsVirtuallyVisible()
        {
            this.Root.Visibility = this.IsVirtuallyVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool IsVirtuallyVisible
        {
            get
            {
                return (bool)base.GetValue(IsVirtuallyVisibleProperty);
            }
            set
            {
                base.SetValue(IsVirtuallyVisibleProperty, value);
            }
        }
    }
}
