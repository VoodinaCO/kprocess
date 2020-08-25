using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Permet d'appeler une commande lorsqu'une ligne d'un datagrid a été double cliquée. Le paramètre de la commande correspond à la valeur liée à la ligne
    /// </summary>
    public class DataGridRowDoubleClickCommandBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(DataGridRowDoubleClickCommandBehavior), new PropertyMetadata());
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(DataGridRowDoubleClickCommandBehavior), new PropertyMetadata());

        /// <summary>
        /// Determine la commande à lancer au double clique
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Surcharge le DataContext de la ligne avec un paramètre custom
        /// </summary>
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Determine le DataContexte prévu par la ligne. Utile lorsque l'enfant cliqué n'est pas une ligne de DataGrid
        /// </summary>
        public Type ExpectedDataContext { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.AddHandler(Control.MouseDoubleClickEvent, new RoutedEventHandler(this.OnDataGridDoubleClicked));
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.RemoveHandler(Control.MouseDoubleClickEvent, new RoutedEventHandler(this.OnDataGridDoubleClicked));
        }

        private void OnDataGridDoubleClicked(object sender, RoutedEventArgs e)
        {
            var source = e.OriginalSource as DependencyObject;
            if (source != null && this.Command != null)
            {
                FrameworkElement item = null;

                if (ExpectedDataContext != null)
                {
                    item = source.FindAncestorByDataContext(this.ExpectedDataContext);
                }
                
                if(item == null)
                {
                    item = source.FindAncestor<DataGridRow>();
                }

                if (item != null)
                {
                    var parameter = CommandParameter ?? item.DataContext;

                    if (this.Command.CanExecute(parameter))
                    {
                        this.Command.Execute(parameter);
                    }
                }
            }
        }
    }
}
