using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    /// <summary>
    /// Représente une fenêtre enfant.
    /// </summary>
    [TemplatePart(Name = ChildWindow.PART_CloseButton, Type = typeof(ButtonBase))]
    public class ChildWindow : ContentControl, IChildWindow
    {

        #region Constantes

        private const string PART_CloseButton = "PART_CloseButton";

        #endregion

        #region Champs privés

        private ButtonBase _closeButton;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialise la classe <see cref="ChildWindow"/>.
        /// </summary>
        static ChildWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChildWindow), new FrameworkPropertyMetadata(typeof(ChildWindow)));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Obtient ou définit le titre de la fenêtre.
        /// </summary>
        public object Title
        {
            get { return (object)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="Title"/>.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(ChildWindow), new UIPropertyMetadata(null));

        /// <summary>
        /// Obtient ou définit le pinceau appliqué à l'overlay.
        /// </summary>
        public Brush OverlayBrush
        {
            get { return (Brush)GetValue(OverlayBrushProperty); }
            set { SetValue(OverlayBrushProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="OverlayBrush"/>.
        /// </summary>
        public static readonly DependencyProperty OverlayBrushProperty =
            DependencyProperty.Register("OverlayBrush", typeof(Brush), typeof(ChildWindow), new UIPropertyMetadata(null));

        /// <summary>
        /// Obtient ou définit uen valeur indiquant si la fenêtre a un bouton de fermeture.
        /// </summary>
        public bool HasCloseButton
        {
            get { return (bool)GetValue(HasCloseButtonProperty); }
            set { SetValue(HasCloseButtonProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="HasCloseButton"/>.
        /// </summary>
        public static readonly DependencyProperty HasCloseButtonProperty =
            DependencyProperty.Register("HasCloseButton", typeof(bool), typeof(ChildWindow), new UIPropertyMetadata(true));

        /// <summary>
        /// Obtient ou définit une valeur indiquant si le paneau de drag & drop de fenêtre est affiché.
        /// </summary>
        public bool HasDragWindowPanel
        {
            get { return (bool)GetValue(HasDragWindowPanelProperty); }
            set { SetValue(HasDragWindowPanelProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="HasDragWindowPanel"/>.
        /// </summary>
        public static readonly DependencyProperty HasDragWindowPanelProperty =
            DependencyProperty.Register("HasDragWindowPanel", typeof(bool), typeof(ChildWindow), new UIPropertyMetadata(false));

        /// <summary>
        /// Obtient ou définit le résultat.
        /// </summary>
        public bool? DialogResult { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Survient lorsque la fenêtre est fermée.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Survient lorsque la fenêtre est en train d'être fermée.
        /// </summary>
        public event CancelEventHandler Closing;

        #endregion

        #region Overrides

        /// <summary>
        /// Applique le ControlTemplate du contrôle.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_closeButton != null)
                _closeButton.Click -= new RoutedEventHandler(OnCloseButtonClick);

            _closeButton = base.GetTemplateChild("PART_CloseButton") as ButtonBase;

            if (_closeButton != null)
                _closeButton.Click += new RoutedEventHandler(OnCloseButtonClick);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Appelé lorsque la fenêtre est affichée.
        /// </summary>
        public virtual void OnShown()
        {
        }

        /// <summary>
        /// Ferme la fenêtre.
        /// </summary>
        public void Close()
        {
            var ea = new CancelEventArgs();
            RaiseClosing(ea);
            if (!ea.Cancel)
                RaiseClosed();
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Appelé lorsque la fenêtre est sur le point d'être fermée.
        /// </summary>
        /// <param name="e">Les <see cref="System.ComponentModel.CancelEventArgs"/> contenant les données de l'évènement.</param>
        protected virtual void OnClosing(CancelEventArgs e)
        {
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Appelé lorsque le bouton de fermeture est cliqué.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = null;
            Close();
        }

        /// <summary>
        /// Lève l'évènement <see cref="E:Closing"/>
        /// </summary>
        /// <param name="e">Les <see cref="System.ComponentModel.CancelEventArgs"/> contenant les données de l'évènement.</param>
        private void RaiseClosing(CancelEventArgs e)
        {
            OnClosing(e);
            if (!e.Cancel && Closing != null)
                Closing(this, e);
        }

        /// <summary>
        /// Lève l'évènement <see cref="E:Closed"/>
        /// </summary>
        private void RaiseClosed()
        {
            if (Closed != null)
                Closed(this, EventArgs.Empty);
        }

        #endregion

    }
}
