using Kprocess.KL2.TabletClient.Converter;
using Kprocess.KL2.TabletClient.ViewModel;
using System;

namespace Kprocess.KL2.TabletClient.Dialog
{
    public partial class FormationActionDetailsDialog
    {
        public FormationActionDetailsDialog()
        {
            InitializeComponent();

            Loaded += View_Loaded;
            Unloaded += View_Unloaded;
        }

        private void View_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is FormationViewModel vm)
            {
                vm.PropertyChanged += Vm_PropertyChanged;
                if (vm.PublishedAction?.CutVideoHash != null)
                {
                    var converter = new HashToUriConverter();
                    Player.Open((Uri)converter.Convert(vm.PublishedAction.CutVideoHash, typeof(Uri), null, null));
                }
            }
        }

        private void View_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is FormationViewModel vm)
            {
                vm.PropertyChanged -= Vm_PropertyChanged;
            }
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is FormationViewModel vm && e.PropertyName == nameof(vm.PublishedAction) && vm.PublishedAction?.CutVideoHash != null)
            {
                var converter = new HashToUriConverter();
                Player.Open((Uri)converter.Convert(vm.PublishedAction.CutVideoHash, typeof(Uri), null, null));
            }
        }
    }
}
