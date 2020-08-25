using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Input;

namespace Kprocess.KL2.TabletClient.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class InspectionTypeChoiceViewModel : ViewModelBase
    {
        ICommand _navigateToInspectionType;
        public ICommand NavigateToInspectionType
        {
            get
            {
                return _navigateToInspectionType ?? (_navigateToInspectionType = new RelayCommand<bool>(async value =>
                {
                    if (value) // Scheduled
                    {
                        var viewModel = new InspectionScheduledViewModel()
                        {
                            IsReadOnly = true
                        };
                        await Locator.Navigation.Push<Views.InspectionScheduledView, InspectionScheduledViewModel>(viewModel);
                    }
                    else // Free
                    {
                        var viewModel = new InspectionChoiceViewModel();
                        await Locator.Navigation.Push<Views.InspectionChoice, InspectionChoiceViewModel>(viewModel);
                    }
                }));
            }
        }

        ICommand _returnCommand;
        public ICommand ReturnCommand
        {
            get
            {
                if (_returnCommand == null)
                    _returnCommand = new RelayCommand(async () =>
                    {
                        await Locator.Navigation.Pop();
                    });
                return _returnCommand;
            }
        }
    }
}