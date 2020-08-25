using DlhSoft.Windows.Controls;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.Shell.ViewModels
{
    public abstract partial class ActionsGraphManipulator
    {
        /// <summary>
        /// Virtualise une liste d'actions sous la forme d'un arbre hiérarchique
        /// </summary>
        /// <param name="items">La liste plate des éléments à virtualiser</param>
        /// <returns>L'arbre virtuel des actions</returns>
        public static ActionsGraphManipulator<TActionItem> Virtualize<TActionItem>(IEnumerable<TActionItem> items)
            where TActionItem : DataTreeGridItem, IActionItem
        {
            return new ActionsGraphManipulator<TActionItem>(items);
        }
    }

    /// <summary>
    /// Permet de virtualiser la hiérarchie des actions au sein d'un Gantt
    /// </summary>
    public partial class ActionsGraphManipulator<TActionItem> : ActionsGraphManipulator
        where TActionItem : DataTreeGridItem, IActionItem
    {
        private IEnumerable<TActionItem> _itemsSource = null;

        internal ActionsGraphManipulator(IEnumerable<TActionItem> items)
        {
            _itemsSource = items;
            this.Root = new RootNode(this);
            var flatList = new List<TActionItem>(items);

            var currentHost = this.Root;
            var currentIndentation = 0;
            foreach (var gridAction in flatList)
            {
                if (gridAction.Indentation < currentIndentation)
                {
                    for (var diff = currentIndentation - gridAction.Indentation; diff > 0; diff--)
                    {
                        // TODO: add a trace if currentHost.Parent == null
                        currentHost = currentHost.Parent ?? this.Root;
                    }
                }

                if (gridAction.IsGroup.GetValueOrDefault())
                {
                    var group = new GroupNode(gridAction, currentHost);
                    currentHost.Items.Add(group);
                    currentHost = group;
                }
                else
                {
                    var leaf = new LeafNode(gridAction, currentHost);
                    currentHost.Items.Add(leaf);
                }

                currentIndentation = gridAction.Indentation;
            }
        }

        /// <summary>
        /// Obtient la liste des éléments à la racine de l'arbre virtuel
        /// </summary>
        public INodeContainer Root { get; private set; }

        public INodeContainer Parent
        {
            get
            {
                return null;
            }
            protected set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Retrouve un noeud virtuel dans le graph à partir de l'index de la liste plate.
        /// </summary>
        /// <param name="index">L'index de la liste plate des éléments</param>
        /// <returns>Le noeud correspondant dans le graph virtuel</returns>
        public Node ResolveNodeByFlatIndex(int index)
        {
            return ResolveNodeByFlatIndex(index, this.Root.Items.FirstOrDefault());
        }

        /// <summary>
        /// Convertie le graph courant en une liste mise à plat
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TActionItem> BuildFlatList()
        {
            ApplyIndentations();
            ApplyWBS();

            var flatList = new List<TActionItem>();
            ProjectGraphToFlatList(from: this.Root, inList: flatList);
            return flatList;
        }

        /// <summary>
        /// Applique les modifications apportées au graph à la liste spécifiée
        /// </summary>
        /// <typeparam name="TComponentItem"></typeparam>
        /// <param name="toExistingList"></param>
        /// <param name="delete">Action to perform when item is deleted</param>
        public void Apply<TComponentItem>(IList<TComponentItem> toExistingList, Action<TActionItem> delete = null)
            where TComponentItem : DataTreeGridItem
        {
            ApplyIndentations();
            ApplyWBS();

            var flatList = new List<TActionItem>();
            ProjectGraphToFlatList(from: this.Root, inList: flatList);

            for(var index = 0; index < flatList.Count; index++)
            {
                var item = flatList[index];

                // TODO: gérer les types qui ne sont pas TActionItem!!!
                // Gestion des éléments supprimés depuis la nouvelle liste
                while (!flatList.Contains(toExistingList[index] as TActionItem))
                {
                    var itemToRemove = toExistingList[index] as TActionItem;
                    itemToRemove.Action.MarkAsDeleted();
                    toExistingList.RemoveAt(index);
                    (delete ?? delegate { })(itemToRemove);
                }

                // Si à l'index courant l'element ne s'y trouve pas, on déplace à la bonne position
                if(toExistingList[index] != item)
                {
                    toExistingList.Remove(item as TComponentItem);
                    toExistingList.Insert(index, item as TComponentItem); // Le cas d'insertion à la dernière place ne peut logiquement pas se produire vu que le dernier élément est nécessairement à la bonne place
                }
            }

        }

        #region private methods
        /// <summary>
        /// Convertie de façon récursive le graphe en une liste plate d'élements
        /// </summary>
        /// <param name="from"></param>
        /// <param name="inList"></param>
        private static void ProjectGraphToFlatList(INodeContainer from, IList<TActionItem> inList)
        {
            var currentContainer = from;

            foreach (var node in currentContainer.Items)
            {
                inList.Add(node.Content);

                if (node is INodeContainer)
                {
                    ProjectGraphToFlatList((INodeContainer)node, inList);
                }
            }
        }

        private void ApplyWBS(INodeContainer from = null, IList<int> atLevels = null)
        {
            var currentContainer = from ?? this.Root;
            var currentLevels = new List<int>(atLevels ?? new[] { WBSHelper.GetFirstIndentation() });

            foreach (var node in currentContainer.Items)
            {
                node.Content.Action.WBS = WBSHelper.LevelsToWBS(currentLevels);

                if (node is INodeContainer)
                {
                    var subLevel = new List<int>(currentLevels);
                    subLevel.Add(WBSHelper.GetFirstIndentation());
                    ApplyWBS((INodeContainer)node, subLevel);
                }

                currentLevels[currentLevels.Count - 1]++;
            }
        }

        private void ApplyIndentations(INodeContainer from = null, int atLevel = 0)
        {
            var currentContainer = from ?? this.Root;
            var currentIndex = atLevel;
            if (currentContainer == null)
            {
                return;
            }

            foreach (var node in currentContainer.Items)
            {
                node.Content.Indentation = atLevel;

                if (node is INodeContainer)
                {
                    ApplyIndentations((INodeContainer)node, atLevel + 1);
                }
            }
        }

        private static Node ResolveNodeByFlatIndex(int index, Node fromNode, int fromIndex = 0)
        {
            var currentNode = fromNode;
            var currentIndex = fromIndex;

            if (fromIndex >= index) // Fin de la récursivité
            {
                return currentNode;
            }

            Node nextNode = null;
            if (currentNode is INodeContainer && ((INodeContainer)currentNode).Items.Any())
            {
                nextNode = ((INodeContainer)currentNode).Items.First();
            }
            else if (currentNode.Parent.Items.Last() == currentNode)
            {
                // On cherche le parent qui contient encore des enfant après
                var nextNodeParentReference = FindParentAtNotLastIndex(currentNode.Parent);
                if (nextNodeParentReference == null)
                {
                    return null; // Fin du graph
                }

                var parentReferenceIndex = nextNodeParentReference.Parent.Items.IndexOf((Node)nextNodeParentReference);
                nextNode = nextNodeParentReference.Parent.Items[parentReferenceIndex + 1];
            }
            else
            {
                var container = currentNode.Parent;
                nextNode = container.Items[container.Items.IndexOf(currentNode) + 1];
            }

            return ResolveNodeByFlatIndex(index, nextNode, currentIndex + 1);
        }

        /// <summary>
        /// Recherche récursivement un noeud parent qui ne soit pas le dernier de son groupe
        /// </summary>
        /// <param name="fromContainer"></param>
        /// <returns></returns>
        private static INodeContainer FindParentAtNotLastIndex(INodeContainer fromContainer)
        {
            if (fromContainer.IsRoot)
            {
                return null;
            }

            var currentParent = fromContainer;
            var parentContainer = currentParent.Parent;
            if (parentContainer.Items.Last() != currentParent)
            {
                return currentParent;
            }

            return FindParentAtNotLastIndex(parentContainer);
        }

        /// <summary>
        /// Recherche le noeud le plus bas du graph
        /// </summary>
        /// <param name="from"></param>
        /// <returns>Le noeud le plus bas ou null si le conteneur courant est vide</returns>
        private Node FindLastNode(INodeContainer from = null)
        {
            var currentContainer = from ?? this.Root;
            
            if(!currentContainer.Items.Any())
            {
                return null;
            }

            var lastNode = currentContainer.Items.Last();
            if(lastNode is INodeContainer)
            {
                return FindLastNode((INodeContainer)lastNode);
            }

            return lastNode;
        }
        #endregion
    }
}
