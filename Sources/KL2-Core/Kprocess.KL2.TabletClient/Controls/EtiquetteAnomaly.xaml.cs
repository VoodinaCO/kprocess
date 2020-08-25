using GalaSoft.MvvmLight.CommandWpf;
using Kprocess.KL2.TabletClient.Dialog;
using Kprocess.KL2.TabletClient.ViewModel;
using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Kprocess.KL2.TabletClient.Controls
{
    /// <summary>
    /// Logique d'interaction pour EtiquetteAnomaly.xaml
    /// </summary>
    public partial class EtiquetteAnomaly : UserControl
    {
        static double _defaultRowHeight = 26;

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(EtiquetteAnomaly), new PropertyMetadata(false));
        public static readonly DependencyProperty KindProperty = DependencyProperty.Register(nameof(Kind), typeof(AnomalyType), typeof(EtiquetteAnomaly), new PropertyMetadata(AnomalyType.Security, OnKindPropertyChanged));
        public static readonly DependencyProperty PossibleAnomaliesProperty = DependencyProperty.Register(nameof(PossibleAnomalies), typeof(List<IAnomalyKindItem>), typeof(EtiquetteAnomaly), new PropertyMetadata(Anomalies.GetPossibleAnomalies(AnomalyType.Security)));
        public static readonly DependencyProperty DarkColorProperty = DependencyProperty.Register(nameof(DarkColor), typeof(SolidColorBrush), typeof(EtiquetteAnomaly), new PropertyMetadata(Application.Current.Resources["DarkColorSecurity"] as SolidColorBrush));
        public static readonly DependencyProperty RowHeightProperty = DependencyProperty.Register(nameof(RowHeight), typeof(double), typeof(EtiquetteAnomaly), new PropertyMetadata(_defaultRowHeight));
        public static readonly DependencyProperty PanelHeightProperty = DependencyProperty.Register(nameof(PanelHeight), typeof(double), typeof(EtiquetteAnomaly), new PropertyMetadata(_defaultRowHeight * 5));

        public static readonly DependencyProperty AnomalyProperty = DependencyProperty.Register(nameof(Anomaly), typeof(Anomaly), typeof(EtiquetteAnomaly), new PropertyMetadata(null));
        public static readonly DependencyProperty SelectedPriorityProperty = DependencyProperty.Register(nameof(SelectedPriority), typeof(AnomalyPriority), typeof(EtiquetteAnomaly), new PropertyMetadata(AnomalyPriority.A));
        public static readonly DependencyProperty SelectedAnomalyKindItemProperty = DependencyProperty.Register(nameof(SelectedAnomalyKindItem), typeof(IAnomalyKindItem), typeof(EtiquetteAnomaly), new PropertyMetadata(null));
        public static readonly DependencyProperty LineProperty = DependencyProperty.Register(nameof(Line), typeof(string), typeof(EtiquetteAnomaly), new PropertyMetadata(null));
        public static readonly DependencyProperty MachineProperty = DependencyProperty.Register(nameof(Machine), typeof(string), typeof(EtiquetteAnomaly), new PropertyMetadata(null));
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(nameof(Label), typeof(string), typeof(EtiquetteAnomaly), new PropertyMetadata(null));
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string), typeof(EtiquetteAnomaly), new PropertyMetadata(null));
        public static readonly DependencyProperty CaptureStreamProperty = DependencyProperty.Register(nameof(CaptureStream), typeof(MemoryStream), typeof(EtiquetteAnomaly), new PropertyMetadata(null));
        public static readonly DependencyProperty RetryPhotoCommandProperty = DependencyProperty.Register(nameof(RetryPhotoCommand), typeof(ICommand), typeof(EtiquetteAnomaly), new PropertyMetadata(null));

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public AnomalyType Kind
        {
            get => (AnomalyType)GetValue(KindProperty);
            set => SetValue(KindProperty, value);
        }
        static void OnKindPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EtiquetteAnomaly etiquette = (EtiquetteAnomaly)d;
            if (e.NewValue is AnomalyType anomalyType)
            {
                switch (anomalyType)
                {
                    case AnomalyType.Security:
                        etiquette.DarkColor = Application.Current.Resources["DarkColorSecurity"] as SolidColorBrush;
                        etiquette.PanelHeight = etiquette.RowHeight * 5;
                        break;
                    case AnomalyType.Maintenance:
                        etiquette.DarkColor = Application.Current.Resources["DarkColorMaintenance"] as SolidColorBrush;
                        etiquette.PanelHeight = etiquette.RowHeight * 9;
                        break;
                    case AnomalyType.Operator:
                        etiquette.DarkColor = Application.Current.Resources["DarkColorOperator"] as SolidColorBrush;
                        etiquette.PanelHeight = etiquette.RowHeight * 9;
                        break;
                }
                etiquette.PossibleAnomalies = Anomalies.GetPossibleAnomalies(anomalyType);
            }
        }

        public List<IAnomalyKindItem> PossibleAnomalies
        {
            get => (List<IAnomalyKindItem>)GetValue(PossibleAnomaliesProperty);
            set => SetValue(PossibleAnomaliesProperty, value);
        }

        public SolidColorBrush DarkColor
        {
            get => (SolidColorBrush)GetValue(DarkColorProperty);
            private set => SetValue(DarkColorProperty, value);
        }

        public double RowHeight
        {
            get => (double)GetValue(RowHeightProperty);
            private set => SetValue(RowHeightProperty, value);
        }

        public double PanelHeight
        {
            get => (double)GetValue(PanelHeightProperty);
            private set => SetValue(PanelHeightProperty, value);
        }

        public Anomaly Anomaly
        {
            get => (Anomaly)GetValue(AnomalyProperty);
            set => SetValue(AnomalyProperty, value);
        }

        public AnomalyPriority SelectedPriority
        {
            get => (AnomalyPriority)GetValue(SelectedPriorityProperty);
            set => SetValue(SelectedPriorityProperty, value);
        }

        public IAnomalyKindItem SelectedAnomalyKindItem
        {
            get => (IAnomalyKindItem)GetValue(SelectedAnomalyKindItemProperty);
            set => SetValue(SelectedAnomalyKindItemProperty, value);
        }

        public string Line
        {
            get => (string)GetValue(LineProperty);
            set => SetValue(LineProperty, value);
        }

        public string Machine
        {
            get => (string)GetValue(MachineProperty);
            set => SetValue(MachineProperty, value);
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            private set => SetValue(LabelProperty, value);
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public MemoryStream CaptureStream
        {
            get => (MemoryStream)GetValue(CaptureStreamProperty);
            set => SetValue(CaptureStreamProperty, value);
        }

        public ICommand RetryPhotoCommand
        {
            get => (ICommand)GetValue(RetryPhotoCommandProperty);
            set => SetValue(RetryPhotoCommandProperty, value);
        }

        ICommand _showTextDialogCommand;
        public ICommand ShowTextDialogCommand
        {
            get
            {
                if (_showTextDialogCommand == null)
                {
                    _showTextDialogCommand = new RelayCommand(async () =>
                    {
                        if (DataContext is AddInspectionAnomalyViewModel addInspectionAnomalyViewModel && !IsReadOnly)
                        {
                            addInspectionAnomalyViewModel.TextDialogResult = Description;
                            await Locator.Navigation.PushDialog<TextDialog, AddInspectionAnomalyViewModel>(addInspectionAnomalyViewModel);
                        }
                    });
                }
                return _showTextDialogCommand;
            }
        }

        public EtiquetteAnomaly()
        {
            InitializeComponent();
        }

        void Priority_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsReadOnly)
                return;
            if (sender is Border border && border.Tag is AnomalyPriority priority)
                SelectedPriority = priority;
        }

        void AnomalyKindItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsReadOnly)
                return;
            if (sender is Border border && border.Tag is IAnomalyKindItem bAnomalyKindItem)
                SelectedAnomalyKindItem = bAnomalyKindItem;
            else if (sender is TextBlock textBlock && textBlock.Tag is IAnomalyKindItem tAnomalyKindItem)
                SelectedAnomalyKindItem = tAnomalyKindItem;
        }

        void ValueTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Tag is IAnomalyKindItem anomalyKindItem)
                SelectedAnomalyKindItem = anomalyKindItem;
        }
    }
}
