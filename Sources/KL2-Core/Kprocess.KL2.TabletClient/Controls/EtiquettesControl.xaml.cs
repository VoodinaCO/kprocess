using KProcess.Ksmed.Models;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kprocess.KL2.TabletClient.Controls
{
    /// <summary>
    /// Logique d'interaction pour EtiquettesControl.xaml
    /// </summary>
    public partial class EtiquettesControl : UserControl
    {
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(EtiquettesControl), new PropertyMetadata(false));
        public static readonly DependencyProperty SelectedKindProperty = DependencyProperty.Register(nameof(SelectedKind), typeof(AnomalyType), typeof(EtiquettesControl), new PropertyMetadata(AnomalyType.Security));
        public static readonly DependencyProperty CaptureStreamProperty = DependencyProperty.Register(nameof(CaptureStream), typeof(MemoryStream), typeof(EtiquettesControl), new PropertyMetadata(null));
        public static readonly DependencyProperty RetryPhotoCommandProperty = DependencyProperty.Register(nameof(RetryPhotoCommand), typeof(ICommand), typeof(EtiquettesControl), new PropertyMetadata(null));

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public AnomalyType SelectedKind
        {
            get => (AnomalyType)GetValue(SelectedKindProperty);
            set => SetValue(SelectedKindProperty, value);
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

        public EtiquettesControl()
        {
            InitializeComponent();
        }

        void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                switch (border.Name)
                {
                    case nameof(SecurityBorder):
                        SelectedKind = AnomalyType.Security;
                        break;
                    case nameof(MaintenanceBorder):
                        SelectedKind = AnomalyType.Maintenance;
                        break;
                    case nameof(OperatorBorder):
                        SelectedKind = AnomalyType.Operator;
                        break;
                }
            }
        }
    }
}
