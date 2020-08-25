using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Permet de sauvegarder les dimensions des lignes et des colonnes d'une grille.
    /// </summary>
    public class GridLayoutPersistanceBehavior : LoadedBehavior<Grid>
    {
        private IServiceBus _serviceBus;
        private string _gridName;

        /// <summary>
        /// Appelé une fois que le comportement est attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            _serviceBus = IoC.Resolve<IServiceBus>();
        }

        /// <summary>
        /// Appelé lorsque l'objet attaché a été chargé.
        /// </summary>
        protected override void OnLoaded()
        {
            base.OnLoaded();

            this.ComputeGridName();
            this.Restore();
        }

        /// <summary>
        /// Appelé lorsque le comportement est détaché de son AssociatedObject, mais avant qu'il ne se soit produit réellement.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.Persist();
        }

        /// <summary>
        /// Calcule le nom de la grille.
        /// </summary>
        private void ComputeGridName()
        {
            var name = new StringBuilder();

            DependencyObject parent = base.AssociatedObject;
            while (parent != null)
            {
                name.Append(parent.GetType().Name);
                parent = parent.TryFindParent<DependencyObject>();
            }

            _gridName = name.ToString();
        }

        /// <summary>
        /// Restaure le layout de la grille à partir du layout sauvegardé.
        /// </summary>
        private void Restore()
        {
            GridLength[] columns;
            GridLength[] rows;
            if (_serviceBus.Get<ILayoutPersistanceService>().GridTryRetrieve(_gridName, out columns, out rows))
            {
                if (this.AssociatedObject.ColumnDefinitions.Count == columns.Length)
                {
                    int i = 0;
                    foreach (var columnLength in columns)
                    {
                        this.AssociatedObject.ColumnDefinitions[i].Width = columnLength;
                        i++;
                    }
                }

                if (this.AssociatedObject.RowDefinitions.Count == rows.Length)
                {
                    int i = 0;
                    foreach (var rowLength in rows)
                    {
                        this.AssociatedObject.RowDefinitions[i].Height = rowLength;
                        i++;
                    }
                }
            }
        }

        /// <summary>
        /// Persiste le layout de la grille.
        /// </summary>
        private void Persist()
        {
            _serviceBus.Get<ILayoutPersistanceService>().GridPersist(_gridName, this.AssociatedObject.ColumnDefinitions.Select(c => c.Width), this.AssociatedObject.RowDefinitions.Select(r => r.Height));
        }

    }
}
