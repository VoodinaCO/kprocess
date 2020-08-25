using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using KProcess.Presentation.Windows;
using Microsoft.Expression.Interactivity.Layout;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Représente l'implémentation par défaut du service affichant des fenêtres enfants.
    /// </summary>
    public class ChildWindowService : IChildWindowService, IViewHandleService
    {
        #region IChildWindowService Members

        private bool _isChildWindowOpened;
        private Func<bool> _onClosing;
        private Action<bool?> _onClosed;
        private IChildWindow _childWindow;
        private ChildWindowAdorner _adorner;
        private Window _parentWindow;

        /// Afficher une fenêtre enfant.
        /// </summary>
        /// <param name="source">L'objet source. Il doit appartenir à une fenêtre affichée.</param>
        /// <param name="childWindow">La fenêtre enfant à afficher.</param>
        /// <param name="onClosed">Un délégué appelé lorsque la fenêtre est sur le point d'être fermée. Définir la valeur de retour à <c>true</c> pour annuler la fermeture.</param>
        /// <param name="onClosed">Un délégué appelé lorsque la fenêtre est fermée.</param>
        public void ShowDialog(DependencyObject source, IChildWindow childWindow, Func<bool> onClosing = null, Action<bool?> onClosed = null)
        {
            Assertion.NotNull(source, "source");
            Assertion.NotNull(childWindow, "window");

            if (_isChildWindowOpened)
                throw new Exception("A child window cannot be opened because there is already one displayed");

            var parentWindow = source as Window ?? source.TryFindParent<Window>();
            if (parentWindow == null)
            {
                throw new ArgumentException("The parent window of the source argument cannot be found", "source");
            }

            var firstFocusableChild = parentWindow.FindFirstChild<AdornerDecorator>();

            if (firstFocusableChild == null)
                throw new Exception("The child window cannot be shown because the parent window doesn't have any adorner decorator");

            _isChildWindowOpened = true;
            _onClosing = onClosing;
            _onClosed = onClosed;
            _childWindow = childWindow;
            _parentWindow = parentWindow;

            _childWindow.Closing += new CancelEventHandler(OnChildWindow_Closing);
            _childWindow.Closed += new EventHandler(OnChildWindow_Closed);
            _parentWindow.Closed += new EventHandler(OnParentWindow_Closed);
            Application.Current.Exit += new ExitEventHandler(OnApplicationExit);

            _adorner = new ChildWindowAdorner(firstFocusableChild.Child, childWindow);

            childWindow.OnShown();
        }

        /// <summary>
        /// Appelé lorsque la fenêtre enfant est sur le point d'être fermée.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Les <see cref="System.ComponentModel.CancelEventArgs"/> contenant les données de l'évènement.</param>
        private void OnChildWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_onClosing != null)
                e.Cancel = _onClosing();
        }

        /// <summary>
        /// Appelé lorsque la fenêtre enfant a été fermée.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.EventArgs"/> contenant les données de l'évènement.</param>
        private void OnChildWindow_Closed(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Appelé lorsque la fenêtre parente a été fermée.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.EventArgs"/> contenant les données de l'évènement.</param>
        private void OnParentWindow_Closed(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Appelé lorsque l'application a été quittée.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.Windows.ExitEventArgs"/> contenant les données de l'évènement.</param>
        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Ferme la fenêtre enfant.
        /// </summary>
        private void Close()
        {
            if (_childWindow != null && _onClosed != null)
                _onClosed(_childWindow.DialogResult);

            if (_adorner != null)
                _adorner.Detach();

            _isChildWindowOpened = false;

            if (_childWindow != null)
            {
                _childWindow.Closed -= new EventHandler(OnChildWindow_Closed);
                _childWindow.Closing -= new CancelEventHandler(OnChildWindow_Closing);
            }
            if (_parentWindow != null)
                _parentWindow.Closed -= new EventHandler(OnParentWindow_Closed);
            Application.Current.Exit -= new ExitEventHandler(OnApplicationExit);

            _childWindow = null;
            _onClosing = null;
            _onClosed = null;
            _parentWindow = null;
        }

        /// <summary>
        /// Représente l'adorner d'une fenêtre enfant.
        /// </summary>
        private class ChildWindowAdorner : AdornerContainer
        {
            private AdornerLayer _adornerLayer;

            /// <summary>
            /// Initialise une nouvelle instance de la classe <see cref="ChildWindowAdorner"/>.
            /// </summary>
            /// <param name="adornerElement">L'adorner.</param>
            /// <param name="windowContent">Le contenu de la fenêtre.</param>
            public ChildWindowAdorner(UIElement adornerElement, object windowContent)
                : base(adornerElement)
            {
                _adornerLayer = AdornerLayer.GetAdornerLayer(adornerElement);

                var cp = new ContentPresenter();
                cp.Content = windowContent;
                this.Child = cp;

                _adornerLayer.Add(this);
                _adornerLayer.Update(this.AdornedElement);
            }

            /// <summary>
            /// Détache l'adorner de sont AdornerLayer.
            /// </summary>
            public void Detach()
            {
                this.Child = null;
                _adornerLayer.Remove(this);
            }

        }

        #endregion

        #region IViewHandleService Members

        private Dictionary<ViewModelWeakReference, HandleWeakReference> _references =
            new Dictionary<ViewModelWeakReference, HandleWeakReference>();

        /// <summary>
        /// Enregistre l'association spécifiée.
        /// </summary>
        /// <param name="viewModel">Le VM.</param>
        /// <param name="handle">L'attache.</param>
        public void Register(IViewModel viewModel, DependencyObject handle)
        {
            _references[new ViewModelWeakReference(viewModel)] = new HandleWeakReference(handle);
            Cleanup();
        }

        /// <summary>
        /// Libère les associations donc l'attache est celle spécifiée
        /// </summary>
        /// <param name="handle">L'attache.</param>
        public void Release(DependencyObject handle)
        {
            var firstMatching = _references.Where(r => r.Value.IsAlive && Object.ReferenceEquals(r.Value.Handle, handle)).ToArray();
            if (firstMatching.Any())
                foreach (var kvp in firstMatching)
                    _references.Remove(kvp.Key);

            Cleanup();
        }

        /// <summary>
        /// Obtient l'attache pour le VM spécifié.
        /// </summary>
        /// <param name="viewModel">Le vm.</param>
        /// <returns>
        /// L'attache ou null.
        /// </returns>
        public DependencyObject Resolve(IViewModel viewModel)
        {
            var firstMatching = _references.FirstOrDefault(r =>
                r.Key.IsAlive && r.Value.IsAlive && Object.ReferenceEquals(r.Key.ViewModel, viewModel));
            Cleanup();

            return firstMatching.Value.Handle;
        }

        /// <summary>
        /// Libère les références mortes.
        /// </summary>
        private void Cleanup()
        {
            var copy = _references.ToArray();
            foreach (var kvp in copy)
            {
                if (!kvp.Key.IsAlive && !kvp.Value.IsAlive)
                    _references.Remove(kvp.Key);
            }
        }

        /// <summary>
        /// Représente une référence faible vers un VM.
        /// </summary>
        private class ViewModelWeakReference : WeakReference, IEquatable<ViewModelWeakReference>
        {
            public ViewModelWeakReference(IViewModel viewModel)
                : base(viewModel)
            {
            }

            public IViewModel ViewModel
            {
                get { return (IViewModel)Target; }
            }

            public bool Equals(ViewModelWeakReference other)
            {
                return this.Target == other.Target;
            }
            public override bool Equals(object obj)
            {
                return this.Equals(obj as ViewModelWeakReference);
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        /// <summary>
        /// Représente une référence faible vers une attache.
        /// </summary>
        private class HandleWeakReference : WeakReference
        {
            public HandleWeakReference(DependencyObject handle)
                : base(handle)
            {
            }

            public DependencyObject Handle
            {
                get { return (DependencyObject)Target; }
            }

            public bool Equals(ViewModelWeakReference other)
            {
                return this.Target == other.Target;
            }
            public override bool Equals(object obj)
            {
                return this.Equals(obj as ViewModelWeakReference);
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }


        #endregion
    }
}
