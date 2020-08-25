// -----------------------------------------------------------------------
// <copyright file="CommandOnEventAction.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Behavior permettant de lier une commande à un routedEvent
    /// </summary>
#if SILVERLIGHT
    [DefaultTrigger(typeof(System.Windows.Controls.Control), typeof(System.Windows.Interactivity.EventTrigger), "MouseLeftButtonDown")]
    public class CommandOnEventAction : TargetedTriggerAction<System.Windows.Controls.Control>
#else
    [DefaultTrigger(typeof(FrameworkElement), typeof(System.Windows.Interactivity.EventTrigger), "MouseLeftButtonDown")]
    [System.ComponentModel.Description("Behavior permettant de lier une commande à un routedEvent")]
    public class CommandOnEventAction : TargetedTriggerAction<FrameworkElement>
#endif
    {
        #region Dependency properties

        /// <summary>
        /// Dependency property de la commande
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(CommandOnEventAction), new PropertyMetadata(OnCommandPropertyChanged));

        /// <summary>
        /// Dependency property du paramètre de la commande
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(CommandOnEventAction), new PropertyMetadata(null));

        #endregion

        #region Propriétés

        /// <summary>
        /// Indique si l'eventArgs de l'événement doit être passé en tant que paramètre de la commande
        /// </summary>
        public bool PassEventArgsToCommand { get; set; }
        
        /// <summary>
        /// Indique si le frameworkElement doit voir sa propriété IsEnabled synchronisée avec la capacité d'exécution de la commande
        /// </summary>
        public bool SyncIsEnabled { get; set; }

        /// <summary>
        /// Obtient ou définit la commande à exécuter lors du déclenchement du routedEvent
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Obtient ou définit le paramètre à passer à la commande
        /// </summary>
        public object CommandParameter
        {
            get { return this.GetValue(CommandParameterProperty); }
            set { this.SetValue(CommandParameterProperty, value); }
        }

        #endregion

        #region Surcharges

        /// <summary>
        /// Invoque l'action
        /// </summary>
        /// <param name="parameter">paramètre fourni</param>
        protected override void Invoke(object parameter)
        {
            if (this.Command != null)
            {
                // Passe le bon paramètre à la commande
                object param = (PassEventArgsToCommand) ? parameter : CommandParameter;

                // Exécute la commande
                if (this.Command.CanExecute(param))
                    this.Command.Execute(param);
            }
        }

        #endregion

        #region Méthode privées

        /// <summary>
        /// Appelé lorsque la valeur de la propriété de dépendance <see cref="CommandProperty"/> a changé.
        /// </summary>
        /// <param name="d">L'objet de dépendance où la propriété a changé.</param>
        /// <param name="o">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs o)
        {
            CommandOnEventAction current = (CommandOnEventAction)d;

            if (current.SyncIsEnabled)
            {
                ICommand oldCommand = o.OldValue as ICommand;
                ICommand newCommand = o.NewValue as ICommand;

                // Désabonnement du handler
                if (o.OldValue != null)
                    oldCommand.CanExecuteChanged -= current.OnCanExecuteChanged;

                // Abonnement au handler
                if (o.NewValue != null)
                    newCommand.CanExecuteChanged += current.OnCanExecuteChanged;
            }
        }

        /// <summary>
        /// Appelé lorsque le CanExecute de la commande associée est susceptible d'avoir changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.EventArgs"/> contenant les données de l'évènement.</param>
        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
            if (Command != null)
                Target.IsEnabled = Command.CanExecute(CommandParameter);
        }

        #endregion
    }
}
