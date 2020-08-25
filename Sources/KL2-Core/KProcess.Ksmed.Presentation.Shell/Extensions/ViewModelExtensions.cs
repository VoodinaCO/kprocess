using KProcess.Globalization;
using KProcess.Presentation.Windows;
using System;

namespace KProcess.Ksmed.Presentation.Shell
{
    /// <summary>
    /// Etend le comportement des ViewModels
    /// </summary>
    public static class ViewModelExtensions
    {
        /// <summary>
        /// Asks validation confirmation and validate the ViewModel if asked.
        /// </summary>
        /// <param name="vm">The ViewMode from which validation is potentiel</param>
        /// <param name="onValidating">Action to be executed just before validation execution</param>
        /// <returns>The final result of the confirmation dialog</returns>
        public static MessageDialogResult AskNavigatingAwayValidationConfirmation(this IViewModel vm, Action onValidating = null)
        {
            var result = IoC.Resolve<IDialogFactory>().GetDialogView<IMessageDialog>().Show(
                    LocalizationManager.GetString("Common_Message_WantToSave"),
                    LocalizationManager.GetString("Common_Confirmation"),
                    MessageDialogButton.YesNoCancel,
                    MessageDialogImage.Question);

            if (result != MessageDialogResult.Yes)
                return result;

            if (!vm.ValidateCommand.CanExecute(null))
                return MessageDialogResult.Cancel;

            onValidating?.Invoke();
            vm.ValidateCommand.Execute(null);
            return MessageDialogResult.Yes;
        }
    }
}
