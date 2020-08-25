using Syncfusion.Windows.Tools.Controls;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{
    /// <summary>
    /// Interface marqueuse permettant de gérer un WizardControl
    /// </summary>
    public interface IWizardViewModel
    {
        WizardControl Wizard { get; set; }
    }
}
