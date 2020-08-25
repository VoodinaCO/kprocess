using KProcess.Presentation.Windows;
using Microsoft.Expression.Interactivity.Layout;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using KProcess.Globalization;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Représente un service d'affichage de spinner.
    /// </summary>
    public class SpinnerService : ISpinnerService
    {
        UIElement _container;
        DataTemplate _spinnerTemplate;
        SpinnerAdorner _adorner;
        string _defaultMessage;
        int _count;

        /// <summary>
        /// Initialise le service.
        /// </summary>
        /// <param name="source">Le conteneur à partir duquel sera affiché le spinner.</param>
        /// <param name="spinnerTemplate">Le DataTemplate utilisé pour afficher le spinner.</param>
        /// <param name="defaultMessage">Le message par défaut à afficher.</param>
        public void Initialize(UIElement source, DataTemplate spinnerTemplate, string defaultMessage)
        {
            var parentWindow = source as Window ?? source.TryFindParent<Window>();
            if (parentWindow == null)
            {
                throw new ArgumentException("The parent window of the source argument cannot be found", nameof(source));
            }

            var firstFocusableChild = parentWindow.FindFirstChild<AdornerDecorator>();

            if (firstFocusableChild == null)
                throw new Exception("The child window cannot be shown because the parent window doesn't have any adorner decorator");

            _container = firstFocusableChild.Child;
            _spinnerTemplate = spinnerTemplate;
            _defaultMessage = defaultMessage;
        }

        /// <summary>
        /// Obtient ou définit l'objet suivant les progressions.
        /// </summary>
        public Progress<double> Progress { get; set; }

        /// <summary>
        /// Obtient ou définit si le spinner est visible.
        /// </summary>
        public bool IsVisible =>
            _count > 0;

        /// <summary>
        /// Affiche le spinner.
        /// </summary>
        /// <param name="message">Le message à afficher.</param>
        public void ShowIncrement(string message = null)
        {
            _count++;
            if (_adorner == null)
                _adorner = new SpinnerAdorner(_container, _spinnerTemplate, message ?? LocalizationManager.GetString(_defaultMessage));
        }

        /// <summary>
        /// Change le message du spinner.
        /// </summary>
        /// <param name="message">Le message à afficher.</param>
        public void SetMessage(string message = null) =>
            _adorner?.SetMessage(message);

        /// <summary>
        /// Cache le spinner.
        /// </summary>
        public void HideDecrement()
        {
            if (_count > 0)
                _count--;

            if (_count == 0)
                Hide();
        }

        /// <summary>
        /// Remet à zéro le compteur, cachant le spinner s'il y en a.
        /// </summary>
        public void Reset()
        {
            _count = 0;
            Hide();
        }

        /// <summary>
        /// Cache le spinner.
        /// </summary>
        void Hide()
        {
            if (_adorner != null)
            {
                _adorner.Detach();
                _adorner = null;
            }
        }

        /// <summary>
        /// Représente l'adorner du spinner.
        /// </summary>
        class SpinnerAdorner : AdornerContainer
        {
            readonly AdornerLayer _adornerLayer;

            /// <summary>
            /// Initialise une nouvelle instance de la classe <see cref="SpinnerAdorner"/>.
            /// </summary>
            /// <param name="adornerElement">L'élement adorné.</param>
            /// <param name="spinnerTemplate">Le DataTemplate du spinner.</param>
            /// <param name="message">Le message.</param>
            public SpinnerAdorner(UIElement adornerElement, DataTemplate spinnerTemplate, string message)
                : base(adornerElement)
            {
                _adornerLayer = AdornerLayer.GetAdornerLayer(adornerElement);

                ContentControl cp = new ContentControl
                {
                    Content = message,
                    ContentTemplate = spinnerTemplate,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                Child = cp;

                _adornerLayer.Add(this);
                _adornerLayer.Update(AdornedElement);
            }

            public void SetMessage(string message = null)
            {
                if (Child is ContentControl cp)
                    cp.Content = message;
            }

            /// <summary>
            /// Détache l'adorner de sont AdornerLayer.
            /// </summary>
            public void Detach()
            {
                _adornerLayer.Remove(this);
            }

        }

    }
}
