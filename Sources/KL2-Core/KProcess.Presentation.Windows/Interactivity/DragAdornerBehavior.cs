using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Controls;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Permet d'ajouter un adorner lors d'une opération de drag &amp; drop.
    /// </summary>
    [StyleTypedProperty(Property = "AdornerStyle", StyleTargetType = typeof(Adorner))]
    [System.ComponentModel.Description("Permet d'ajouter un adorner lors d'une opération de drag &amp; drop.")]
    public class DragAdornerBehavior : Behavior<FrameworkElement>
    {

        private DraggedAdorner _draggedAdorner;
        private Window _topWindow;
        private Point _draggedAdornerOffset;
        private Point _initialMousePosition;

        /// <summary>
        /// Obtient ou définit le DataTemplate appliqué au contenu de l'adorner.
        /// </summary>
        public DataTemplate AdornerDataTemplate
        {
            get { return (DataTemplate)GetValue(AdornerDataTemplateProperty); }
            set { SetValue(AdornerDataTemplateProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="AdornerDataTemplate"/>.
        /// </summary>
        public static readonly DependencyProperty AdornerDataTemplateProperty =
            DependencyProperty.Register("AdornerDataTemplate", typeof(DataTemplate), typeof(DragAdornerBehavior), new UIPropertyMetadata(null));

        /// <summary>
        /// Obtient ou définit le contenu appliqué à l'adorner.
        /// </summary>
        public object AdornerContent
        {
            get { return (object)GetValue(AdornerContentProperty); }
            set { SetValue(AdornerContentProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="AdornerContent"/>.
        /// </summary>
        public static readonly DependencyProperty AdornerContentProperty =
            DependencyProperty.Register("AdornerContent", typeof(object), typeof(DragAdornerBehavior), new UIPropertyMetadata(null));

        /// <summary>
        /// Obtient ou définit le style qui sera appliqué à l'adorner.
        /// </summary>
        public Style AdornerStyle
        {
            get { return (Style)GetValue(AdornerStyleProperty); }
            set { SetValue(AdornerStyleProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="AdornerStyle"/>.
        /// </summary>
        public static readonly DependencyProperty AdornerStyleProperty =
            DependencyProperty.Register("AdornerStyle", typeof(Style), typeof(DragAdornerBehavior), new UIPropertyMetadata(null));

        /// <summary>
        /// Appelé après que le Behavior ait été attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
        }

        /// <summary>
        /// Appelé lorsque le Behavior est en train d'être détaché de l'AssociatedObject.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            RemoveAdorner();
        }

        #region Méthodes publiques

        /// <summary>
        /// Obtient un délégué capable d'obtenir la position du Touch par rapport à un élément.
        /// </summary>
        /// <param name="e">Les données de l'évènement <see cref="System.Windows.Input.TouchEventArgs"/>.</param>
        /// <returns>Le délégué.</returns>
        public Func<IInputElement, Point> GetPositionDelegate(TouchEventArgs e)
        {
            return input => e.GetTouchPoint(input).Position;
        }

        /// <summary>
        /// Obtient un délégué capable d'obtenir la position de la souris par rapport à un élément.
        /// </summary>
        /// <param name="e">Les données de l'évènement <see cref="System.Windows.Input.MouseEventArgs"/>.</param>
        /// <returns>Le délégué.</returns>
        public Func<IInputElement, Point> GetPositionDelegate(MouseEventArgs e)
        {
            return input => e.GetPosition(input);
        }

        /// <summary>
        /// Ajoute l'adorner à l'élément et l'affiche.
        /// </summary>
        /// <param name="positionGetter">Un délégué capable d'obtenir la position de la souris ou du touch. 
        /// Voir <see cref="GetPositionDelegate(TouchEventArgs)"/> et <see cref="GetPositionDelegate(MouseEventArgs)"/>.</param>
        public virtual void AddAdorner(Func<IInputElement, Point> positionGetter)
        {
            Assertion.NotNull(positionGetter, "positionGetter");

            // Get the parent window
            _topWindow = (Window)VisualTreeHelperExtensions.TryFindParent<Window>(this.AssociatedObject);

            if (_topWindow != null)
            {
                // Save the initial position
                _initialMousePosition = positionGetter(_topWindow);

                // Adding events to the window to make sure dragged adorner comes up when mouse is not over a drop target.
                _topWindow.AllowDrop = true;
                _topWindow.PreviewDragEnter += TopWindow_DragEnter;
                _topWindow.PreviewDragOver += TopWindow_DragOver;
            }
        }

        /// <summary>
        /// Supprime l'adorner.
        /// </summary>
        public virtual void RemoveAdorner()
        {
            if (_topWindow != null)
            {
                _topWindow.PreviewDragEnter -= TopWindow_DragEnter;
                _topWindow.PreviewDragOver -= TopWindow_DragOver;

                if (_draggedAdorner != null)
                {
                    _draggedAdorner.Detach();
                    _draggedAdorner = null;
                }

                _topWindow = null;
            }
        }

        #endregion

        /// <summary>
        /// Affiche l'adorner, le crée si nécessaire.
        /// </summary>
        /// <param name="currentPosition">La position relative à la fenêtre parente.</param>
        private void ShowDraggedAdorner(Point currentPosition)
        {
            if (_draggedAdorner == null)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(_topWindow.GetFirstFocusableChild());

                // Créer l'adorner
                if (this.AdornerContent != null || this.AdornerDataTemplate != null)
                    _draggedAdorner = new DraggedAdorner(this.AdornerContent, this.AdornerDataTemplate, this.AssociatedObject, adornerLayer);
                else
                    _draggedAdorner = new DraggedAdorner(CreateAssociatedObjectVisual(), null, this.AssociatedObject, adornerLayer);

                _draggedAdorner.Style = this.AdornerStyle;

                // Obtenir la position
                var transform = base.AssociatedObject.TransformToAncestor(_topWindow) as MatrixTransform;

                var sourcePos = new Point(-transform.Matrix.OffsetX, -transform.Matrix.OffsetY) +
                    new Vector(SystemParameters.CursorWidth / 4, SystemParameters.CursorHeight / 4);

                _draggedAdornerOffset = new Point(this._initialMousePosition.X + sourcePos.X, this._initialMousePosition.Y + sourcePos.Y);
            }

            // mettre à jour la position
            _draggedAdorner.SetPosition(
                currentPosition.X - this._initialMousePosition.X + _draggedAdornerOffset.X,
                currentPosition.Y - this._initialMousePosition.Y + _draggedAdornerOffset.Y);
        }


        /// <summary>
        /// Crée le <see cref="Visual"/> représentant l'<see cref="Behavior.AssociatedObject"/>.
        /// </summary>
        /// <returns>Le <see cref="Visual"/> créé.</returns>
        private Visual CreateAssociatedObjectVisual()
        {
            return new Border()
            {
                Background = new VisualBrush(this.AssociatedObject)
                {
                    Stretch = Stretch.Uniform,
                },
            };
        }

        #region Gestion des évènements sur la fenêtre parente

        /// <summary>
        /// Gère l'évènement DragEnter de la fenêtre parente.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les données de l'évènement.</param>
        private void TopWindow_DragEnter(object sender, DragEventArgs e)
        {
            ShowDraggedAdorner(e.GetPosition(_topWindow));
        }

        /// <summary>
        /// Gère l'évènement DragOver de la fenêtre parente.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les données de l'évènement.</param>
        private void TopWindow_DragOver(object sender, DragEventArgs e)
        {
            ShowDraggedAdorner(e.GetPosition(_topWindow));
        }

        #endregion

        /// <summary>
        /// Adorner contenant l'objet draggé.
        /// </summary>
        private class DraggedAdorner : Adorner
        {
            private ContentPresenter _contentPresenter;
            private double _left;
            private double _top;
            private AdornerLayer _adornerLayer;

            /// <summary>
            /// Obtient le ContentPresenter utilisé pour afficher les données.
            /// </summary>
            public ContentPresenter Presenter { get { return _contentPresenter; } }

            /// <summary>
            /// Constructeur.
            /// </summary>
            /// <param name="dragDropData">Les données affichées dans le ContentPresenter.</param>
            /// <param name="dragDropTemplate">Le DataTemplate utilisé pour afficher les données.</param>
            /// <param name="adornedElement">L'UIElement qui contiendra l'adorner.</param>
            /// <param name="adornerLayer">L'AdornerLayer où l'adorner sera ajouté.</param>
            public DraggedAdorner(object dragDropData, DataTemplate dragDropTemplate, UIElement adornedElement, AdornerLayer adornerLayer)
                : base(adornedElement)
            {
                _adornerLayer = adornerLayer;

                _contentPresenter = new ContentPresenter();
                _contentPresenter.Content = dragDropData;
                _contentPresenter.ContentTemplate = dragDropTemplate;

                _adornerLayer.Add(this);
            }

            /// <summary>
            /// Met à jour la position de l'adorner.
            /// </summary>
            /// <param name="left">Le décalage à gauche.</param>
            /// <param name="top">Le décalage en haut.</param>
            public void SetPosition(double left, double top)
            {
                _left = left;
                _top = top;
                if (_adornerLayer != null)
                {
                    _adornerLayer.Update();
                    _adornerLayer.InvalidateVisual();
                    _adornerLayer.InvalidateMeasure();
                }
                this.InvalidateVisual();
                this.InvalidateMeasure();
                this.InvalidateArrange();

                this.UpdateLayout();
            }

            /// <summary>
            /// Mesure cet élément.
            /// </summary>
            /// <param name="constraint">La taille contrainte.</param>
            /// <returns>
            /// La taille nécessaire pour cette instance.
            /// </returns>
            protected override Size MeasureOverride(Size constraint)
            {
                _contentPresenter.Measure(constraint);
                return _contentPresenter.DesiredSize;
            }

            /// <summary>
            /// Positionne cet élément.
            /// </summary>
            /// <param name="finalSize">La taille finale accordée à cet élément.</param>
            /// <returns>La taille réellement utilisée.</returns>
            protected override Size ArrangeOverride(Size finalSize)
            {
                _contentPresenter.Arrange(new Rect(finalSize));
                return finalSize;
            }

            /// <summary>
            /// Obtient l'enfant à l'index spécifié.
            /// </summary>
            /// <param name="index">L'index.</param>
            /// <returns>
            /// L'enfant à l'index spécifié.
            /// </returns>
            protected override Visual GetVisualChild(int index)
            {
                // return the one and only child
                return _contentPresenter;
            }

            /// <summary>
            /// Obtient le nombre d'enfants de cet élément.
            /// </summary>
            /// <value></value>
            /// <returns>Le nombre d'enfants de cet élément.</returns>
            protected override int VisualChildrenCount
            {
                get { return 1; }
            }

            /// <summary>
            /// Retourne un <see cref="T:System.Windows.Media.Transform"/> appliqué à cet adorner.
            /// </summary>
            /// <param name="transform">La transformation actuellement appliqué à cet adorner.</param>
            /// <returns>La transformation à appliquer..</returns>
            public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
            {
                GeneralTransformGroup result = new GeneralTransformGroup();
                result.Children.Add(base.GetDesiredTransform(transform));
                result.Children.Add(new TranslateTransform(_left, _top));

                return result;
            }

            /// <summary>
            /// Détache l'adorner son layer.
            /// </summary>
            public void Detach()
            {
                _adornerLayer.Remove(this);
            }

        }


    }
}
