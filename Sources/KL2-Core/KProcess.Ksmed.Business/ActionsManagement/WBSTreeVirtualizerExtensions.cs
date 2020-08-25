using KProcess.Ksmed.Business.ActionsManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Business
{
    public static class WBSTreeVirtualizerExtensions
    {
        /// <summary>
        /// Construit un arbre d'<see cref="Models.KAction"/> à partir d'une liste plate d'<see cref="Models.KAction"/>
        /// </summary>
        /// <param name="actions"></param>
        /// <returns></returns>
        public static WBSTreeVirtualizer VirtualizeTree(this IEnumerable<Models.KAction> actions)
        {
            return new WBSTreeVirtualizer(actions);
        }

        /// <summary>
        /// Supprime l'<see cref="Models.KAction"/> spécifiée au sein de l'arbre
        /// </summary>
        /// <param name="virtualizer"></param>
        /// <param name="action">L'<see cref="Models.KAction"/> à supprimer de l'arbre</param>
        /// <returns></returns>
        public static WBSTreeVirtualizer Remove(this WBSTreeVirtualizer virtualizer, Models.KAction action)
        {
            var node = virtualizer.GetNode(action);
            if(node == null)
            {
                throw new InvalidOperationException($"Impossible de résoudre l'action {action.ActionId} dans l'arbre du projet");
            }

            var parent = virtualizer.GetParentNode(action);
            if(parent == null)
            {
                if(!virtualizer.ActionTree.Contains(node))
                {
                    throw new InvalidOperationException($"Impossible de trouver un parent à l'action {action.ActionId}");
                }

                virtualizer.ActionTree.Remove(node);
            }
            else
            {
                parent.Children.Remove(node);
            }

            return virtualizer;
        }
    }
}
