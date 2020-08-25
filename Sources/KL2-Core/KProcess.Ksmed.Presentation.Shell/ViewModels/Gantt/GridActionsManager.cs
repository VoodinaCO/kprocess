using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using DlhSoft.Windows.Controls;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Business.ActionsManagement;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Définit le gestionnaire d'actions pour une grille.
    /// </summary>
    class GridActionsManager : ActionsManager<DataTreeGridItem, ActionGridItem, ReferentialGridItem>
    {
        private BulkObservableCollection<DataTreeGridItem> _items;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="GridActionsManager"/>.
        /// </summary>
        /// <param name="items">La collection des éléments.</param>
        /// <param name="currentItemSetter">Un délégué capable de définir l'élément sélectionné.</param>
        /// <param name="videoGetter">Un délégué capable d'obtenir une vidéo à partir de son identifiant.</param>
        public GridActionsManager(BulkObservableCollection<DataTreeGridItem> items, Action<DataTreeGridItem> currentItemSetter, Func<int, Video> videoGetter)
            : base(currentItemSetter, videoGetter)
        {
            _items = items;
            base.EnableGroupsTimingCoercion = true;
            base.EnableRessourceLoad = false;
            base.EnablePredecessorTimingFix = true;
        }

        /// <summary>
        /// Obtient les éléments.
        /// </summary>
        protected override BulkObservableCollection<DataTreeGridItem> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Crée un nouvel élément représentant une action.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <returns>L'élément créé.</returns>
        protected override ActionGridItem CreateNewActionItemImpl(KAction action)
        {
            if (this.IsViewByReferential())
                return new ActionGridItem(action, WBSHelper.IndentationFromWBS(action.WBS) + 1);
            else
                return new ActionGridItem(action, WBSHelper.IndentationFromWBS(action.WBS));
        }

        /// <inheritdoc />
        protected override ReferentialGridItem CreateNewReferentialItemImpl(IActionReferential referential, string label)
        {
            return new ReferentialGridItem(referential, label);
        }

    }
}
