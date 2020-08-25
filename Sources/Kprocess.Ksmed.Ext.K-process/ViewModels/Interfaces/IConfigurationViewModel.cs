using KProcess.Ksmed.Presentation;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Ext.Kprocess.ViewModels.Interfaces
{
    /// <summary>
    /// Définit le comportement du modèle de vue de l'écran de configuration.
    /// </summary>
    [InheritedExportAsPerCall]
    public interface IConfigurationViewModel : IViewModel, IExtensionConfigurationViewModel
    {
    }
}
