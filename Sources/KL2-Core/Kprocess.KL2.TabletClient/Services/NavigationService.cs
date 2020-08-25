using GalaSoft.MvvmLight;
using Kprocess.KL2.TabletClient.ViewModel;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

// TO DO :
// Créer un IModalNavigationService dans les viewmodels, et les y injecter
// Refactoriser le code
namespace Kprocess.KL2.TabletClient.Services
{
    /// <summary>
    /// Interface pour la navigation entre page
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Méthode permettant de naviguer vers une page
        /// </summary>
        /// <returns></returns>
        Task<TUserControl> Push<TUserControl, TViewModel>(TViewModel viewModel)
            where TUserControl : UserControl, new()
            where TViewModel : ViewModelBase, new();

        /// <summary>
        /// Méthode permettant de revenir vers la page précédente
        /// </summary>
        /// <returns></returns>
        Task Pop();

        /// <summary>
        /// Méthode permettant de revenir vers la page définie
        /// </summary>
        /// <returns></returns>
        Task Pop(params Type[] viewModelTypes);

        /// <summary>
        /// Méthode permettant de récupérer le premier type présent dans la navigation
        /// </summary>
        /// <returns></returns>
        ViewModelBase GetFirstPredecessor(params Type[] viewModelTypes);

        /// <summary>
        /// Méthode permettant de revenir vers la page précédente en retournant un résultat
        /// </summary>
        /// <returns></returns>
        Task PopWithResult<T>(T result);

        /// <summary>
        /// Méthode permettant de naviguer vers une page de type dialog
        /// </summary>
        /// <typeparam name="TUserControl"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="viewModel"></param>
        Task<TUserControl> PushDialog<TUserControl, TViewModel>(TViewModel viewModel)
            where TUserControl : CustomDialog, new()
            where TViewModel : ViewModelBase, new();

        /// <summary>
        /// Méthode permettant de revenir vers la page précédente en mode dialog
        /// </summary>
        Task PopModal();

        /// <summary>
        /// Méthode permettant de revenir sur la page d'accueil
        /// </summary>
        Task ToHome();
    }

    /// <summary>
    /// Implémentation de la navigation entre les pages
    /// </summary>
    public class NavigationService : INavigationService
    {
        #region Attributs

        readonly Stack<ViewStackRecord> _navigationStack = new Stack<ViewStackRecord>();
        readonly Stack<DialogStackRecord> _navigationModalStack = new Stack<DialogStackRecord>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Méthode permettant de naviguer vers une page
        /// </summary>
        /// <returns></returns>
        public async Task<TUserControl> Push<TUserControl, TViewModel>(TViewModel viewModel)
            where TUserControl : UserControl, new()
            where TViewModel : ViewModelBase, new()
        {
            Locator.TraceManager.TraceDebug($"Navigation Push => ViewModelType : {viewModel.GetType()}");
            try
            {
                var view = await Locator.GetView<TUserControl, TViewModel>(viewModel);
                _navigationStack.PushAndLog(new ViewStackRecord(typeof(TUserControl), viewModel));

                Locator.Main.CurrentView = view;

                return view;
            }
            catch (Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Erreur lors de la mise en cache de la vue.");
                return null;
            }
        }

        /// <summary>
        /// Méthode permettant de revenir vers la page précédente
        /// </summary>
        /// <returns></returns>
        public async Task Pop()
        {
            Locator.TraceManager.TraceDebug("Navigation Pop");
            try
            {
                if (_navigationModalStack.Any())
                {
                    // Close the dialog
                    var dialogStackRecord = _navigationModalStack.PopAndLog();
                    if (dialogStackRecord.ViewModel is IMediaElementViewModel && Locator.Main.MediaElement != null)
                    {
                        await Locator.Main.MediaElement.Close();
                        Locator.Main.MediaElement = null;
                    }
                    await Locator.Main.HideMetroDialogAsync(null, dialogStackRecord.Dialog);

                    if (_navigationModalStack.Any()) // Previous item is a dialog
                    {
                        dialogStackRecord = _navigationModalStack.PeekAndLog();
                        if (dialogStackRecord.ViewModel is IRefreshViewModel refreshVM)
                            await refreshVM.Refresh();
                    }
                    else if (_navigationStack.Any()) // Previous is a view
                    {
                        var viewStackRecord = _navigationStack.PeekAndLog();
                        var view = (UserControl)Activator.CreateInstance(viewStackRecord.ViewType);
                        view.DataContext = viewStackRecord.ViewModel;
                        if (view.DataContext is IRefreshViewModel refreshVM)
                            await refreshVM.Refresh();

                        Locator.Main.CurrentView = view;
                    }
                    else // Should never execute this
                    {
                        Locator.Main.CurrentView = null;
                        Locator.TraceManager.TraceWarning("NavigationStack is empty.");
                    }
                }
                else if (_navigationStack.Any())
                {
                    // Close the view
                    var viewStackRecord = _navigationStack.PopAndLog();
                    if (viewStackRecord.ViewModel is IMediaElementViewModel && Locator.Main.MediaElement != null)
                    {
                        await Locator.Main.MediaElement.Close();
                        Locator.Main.MediaElement = null;
                    }

                    // Show previous view
                    if (_navigationStack.Any())
                    {
                        viewStackRecord = _navigationStack.PeekAndLog();
                        var view = (UserControl)Activator.CreateInstance(viewStackRecord.ViewType);
                        view.DataContext = viewStackRecord.ViewModel;
                        if (view.DataContext is IRefreshViewModel refreshVM)
                            await refreshVM.Refresh();

                        Locator.Main.CurrentView = view;
                    }
                    else // Should never execute this
                    {
                        Locator.Main.CurrentView = null;
                        Locator.TraceManager.TraceWarning("NavigationStack is empty.");
                    }
                }
                else // Should never execute this
                {
                    Locator.Main.CurrentView = null;
                    Locator.TraceManager.TraceWarning("NavigationStack is empty.");
                }
            }
            catch (Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Erreur lors de la récupération de la vue précédente.");
            }
        }

        /// <summary>
        /// Méthode permettant de revenir vers la page définie
        /// </summary>
        /// <returns></returns>
        public async Task Pop(params Type[] viewModelTypes)
        {
            Locator.TraceManager.TraceDebug($"Navigation Pop => ViewModelTypes : {string.Join(", ", viewModelTypes.Select(_ => _.ToString()))}");
            try
            {
                if (_navigationStack.Any())
                {
                    ViewStackRecord viewStackRecord;
                    do
                    {
                        viewStackRecord = _navigationStack.PopAndLog();
                        if (viewStackRecord.ViewModel is IMediaElementViewModel && Locator.Main.MediaElement != null)
                        {
                            await Locator.Main.MediaElement.Close();
                            Locator.Main.MediaElement = null;
                        }
                        viewStackRecord = _navigationStack.PeekAndLog();
                        if (_navigationStack.Count <= 1)
                            break;
                    } while (!viewModelTypes.Contains(viewStackRecord.ViewModel.GetType()));

                    var view = (UserControl)Activator.CreateInstance(viewStackRecord.ViewType);
                    view.DataContext = viewStackRecord.ViewModel;
                    if (view.DataContext is IRefreshViewModel refreshVM)
                        await refreshVM.Refresh();

                    Locator.Main.CurrentView = view;
                }
                else // Should never execute this
                {
                    Locator.Main.CurrentView = null;
                    Locator.TraceManager.TraceWarning("NavigationStack is empty.");
                }
            }
            catch (Exception ex)
            {
                Locator.TraceManager.TraceError(ex, $"Erreur lors de la récupération de la vue de ViewModel {string.Join(", ", viewModelTypes.Select(_ => _.ToString()))}.");
            }
        }

        /// <summary>
        /// Méthode permettant de récupérer le premier type présent dans la navigation
        /// </summary>
        /// <returns></returns>
        public ViewModelBase GetFirstPredecessor(params Type[] viewModelTypes)
        {
            var viewStackRecord = _navigationStack.LastOrDefault(_ => viewModelTypes.Contains(_.ViewModel.GetType()));
            return viewStackRecord?.ViewModel;
        }

        /// <summary>
        /// Méthode permettant de revenir vers la page précédente en retournant un résultat
        /// </summary>
        /// <returns></returns>
        public async Task PopWithResult<T>(T result)
        {
            Locator.TraceManager.TraceDebug($"Navigation PopWithResult with result of type {result.GetType()}");
            try
            {
                if (_navigationModalStack.Any())
                {
                    var dialogStackRecord = _navigationModalStack.PopAndLog();
                    if (dialogStackRecord.ViewModel is IMediaElementViewModel && Locator.Main.MediaElement != null)
                    {
                        await Locator.Main.MediaElement.Close();
                        Locator.Main.MediaElement = null;
                    }

                    await Locator.Main.HideMetroDialogAsync(null, dialogStackRecord.Dialog);

                    if (_navigationModalStack.Any()) // Previous item is a dialog
                    {
                        dialogStackRecord = _navigationModalStack.PeekAndLog();
                        if (dialogStackRecord.ViewModel is IWaitResultViewModel<T> waitVM)
                            await waitVM.ComputeResult(result);
                    }
                    else if (_navigationStack.Any()) // Previous item is a view
                    {
                        var viewStackRecord = _navigationStack.PeekAndLog();
                        var view = (UserControl)Activator.CreateInstance(viewStackRecord.ViewType);
                        view.DataContext = viewStackRecord.ViewModel;
                        if (view.DataContext is IWaitResultViewModel<T> waitVM)
                            await waitVM.ComputeResult(result);

                        Locator.Main.CurrentView = view;
                    }
                }
                else if (_navigationStack.Any())
                {
                    var viewStackRecord = _navigationStack.PopAndLog();
                    if (viewStackRecord.ViewModel is IMediaElementViewModel && Locator.Main.MediaElement != null)
                    {
                        await Locator.Main.MediaElement.Close();
                        Locator.Main.MediaElement = null;
                    }

                    if (_navigationStack.Any())
                    {
                        viewStackRecord = _navigationStack.PeekAndLog();
                        var view = (UserControl)Activator.CreateInstance(viewStackRecord.ViewType);
                        view.DataContext = viewStackRecord.ViewModel;
                        if (view.DataContext is IWaitResultViewModel<T> waitVM)
                            await waitVM.ComputeResult(result);

                        Locator.Main.CurrentView = view;
                    }
                }
                else // Should never execute this
                {
                    Locator.Main.CurrentView = null;
                    Locator.TraceManager.TraceWarning("NavigationStack is empty.");
                }
            }
            catch (Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Erreur lors de la récupération de la vue précédente.");
            }
        }

        /// <summary>
        /// Méthode permettant de naviguer vers une page de type dialog
        /// </summary>
        /// <typeparam name="TUserControl"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="viewModel"></param>
        public async Task<TUserControl> PushDialog<TUserControl, TViewModel>(TViewModel viewModel)
            where TUserControl : CustomDialog, new()
            where TViewModel : ViewModelBase, new()
        {
            Locator.TraceManager.TraceDebug($"Navigation PushDialog => DialogType : {typeof(TUserControl)}, ViewModelType : {viewModel.GetType()}");
            try
            {
                var view = new TUserControl { DataContext = viewModel };
                _navigationModalStack.PushAndLog(new DialogStackRecord(view, viewModel));
                await Locator.Main.ShowMetroDialogAsync(null, view);

                return view;
            }
            catch(Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Erreur lors de la mise en cache de la vue modal.");
                return null;
            }
        }

        /// <summary>
        /// Méthode permettant de revenir vers la page précédente en mode dialog
        /// </summary>
        public async Task PopModal()
        {
            Locator.TraceManager.TraceDebug("Navigation PopModal");
            if (!_navigationModalStack.Any())
                return;
            try
            {
                var dialogStackRecord = _navigationModalStack.PopAndLog();
                if (dialogStackRecord.ViewModel is IMediaElementViewModel && Locator.Main.MediaElement != null)
                {
                    await Locator.Main.MediaElement.Close();
                    Locator.Main.MediaElement = null;
                }

                await Locator.Main.HideMetroDialogAsync(null, dialogStackRecord.Dialog);

                if (_navigationStack.Any() && _navigationStack.PeekAndLog().ViewModel is IRefreshViewModel refreshVM)
                    await refreshVM.Refresh();
            }
            catch(Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Erreur lors de la récupération de la vue modal précédente.");
            }
        }

        /// <summary>
        /// Méthode permettant de revenir sur la page d'accueil
        /// </summary>
        public async Task ToHome()
        {
            Locator.TraceManager.TraceDebug("Navigation ToHome");
            try
            {
                _navigationModalStack.Clear();

                while (_navigationStack.Count > 1)
                    _navigationStack.PopAndLog();
                if (_navigationStack.Any())
                {
                    var viewStackRecord = _navigationStack.PeekAndLog();
                    var view = (UserControl)Activator.CreateInstance(viewStackRecord.ViewType);
                    view.DataContext = viewStackRecord.ViewModel;
                    if (view.DataContext is IRefreshViewModel refreshVM)
                        await refreshVM.Refresh();

                    Locator.Main.CurrentView = view;
                }
                else // Should never execute this
                {
                    Locator.Main.CurrentView = null;
                    Locator.TraceManager.TraceWarning("NavigationStack is empty.");
                }
            }
            catch (Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Erreur lors de la navigation vers la page d'accueil.");
            }
        }

        #endregion

        class ViewStackRecord
        {
            public Type ViewType { get; }
            public ViewModelBase ViewModel { get; }

            public ViewStackRecord(Type viewType, ViewModelBase viewModel)
            {
                ViewType = viewType;
                ViewModel = viewModel;
            }

            public override string ToString() =>
                $"ViewType : {ViewType}, ViewModelType : {ViewModel.GetType()}";
        }

        class DialogStackRecord
        {
            public CustomDialog Dialog { get; }
            public ViewModelBase ViewModel { get; }

            public DialogStackRecord(CustomDialog dialog, ViewModelBase viewModel)
            {
                Dialog = dialog;
                ViewModel = viewModel;
            }

            public override string ToString() =>
                $"DialogType : {Dialog.GetType()}, ViewModelType : {ViewModel.GetType()}";
        }
    }

    static class StackLoggingExtension
    {
        public static T PopAndLog<T>(this Stack<T> stack)
        {
            var record = stack.Pop();
            Locator.TraceManager.TraceDebug($"Pop => {record}");
            return record;
        }

        public static T PeekAndLog<T>(this Stack<T> stack)
        {
            var record = stack.Peek();
            Locator.TraceManager.TraceDebug($"Peek => {record}");
            return record;
        }

        public static void PushAndLog<T>(this Stack<T> stack, T record)
        {
            Locator.TraceManager.TraceDebug($"Push => {record}");
            stack.Push(record);
        }
    }
}