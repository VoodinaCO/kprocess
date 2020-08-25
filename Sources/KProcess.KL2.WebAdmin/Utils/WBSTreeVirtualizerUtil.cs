using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Utils
{
    public class WBSTreeVirtualizerUtil<T> where T : IIdentable
    {
        private readonly List<Node> _actionTree;
        public static List<T> _list = new List<T>();

        public List<Node> ActionTree
        {
            get
            {
                return _actionTree;
            }
        }

        public WBSTreeVirtualizerUtil(IEnumerable<T> actionList)
        {
            // _list.AddRange(actionList);
            var splittedActions = actionList.Select(_ => new { splittedWBS = WBSWebActionUtil<T>.GetParts(_.WBS), action = _ }).ToArray();

            var treeRoot = splittedActions.Where(splittedAction => splittedAction.splittedWBS.Length == 1)
                .Select(splittedAction => CreateRecursively(splittedAction.action, actionList))
                .OrderBy(_ => _.Action.WBSParts[0])
                .ToList();

            _actionTree = treeRoot;
        }

        public IEnumerable<T> ApplyWBS()
        {

            for (var index = 0; index < this.ActionTree.Count; index++)
            {
                var action = this.ActionTree[index];
                action.Action.WBS = (index + 1).ToString();
                _list.Add(action.Action);
            }

            this.ActionTree.ForEach(_ => this.ApplyRecursively(_));

            return _list;
        }

        public Node GetNode(T action)
        {
            return this.GetAllNodes().FirstOrDefault(_ => _.Action.Equals(action));
        }

        public Node GetParentNode(T action) => this.GetAllNodes().FirstOrDefault(node => node.Children.Any(_ => _.Action.Equals(action)));

        public static IEnumerable<Node> GetDescendItems(Node node) => node.Children.Union(node.Children.SelectMany(GetDescendItems));

        private IEnumerable<Node> GetAllNodes() => this.ActionTree.Union(this.ActionTree.SelectMany(GetDescendItems));

        public void AddNode(Node node)
        {
            _actionTree.Add(node);
        }

        private Node CreateRecursively(T action, IEnumerable<T> actions)
        {
            var wrappedAction = new Node(action);
            wrappedAction.Children = new List<Node>();
            if (WBSWebActionUtil<T>.HasChildren(action, actions))
            {
                wrappedAction.Children = WBSWebActionUtil<T>.GetChildren(action, actions)
                    .Select(_ => CreateRecursively(_, actions))
                    .OrderBy(_ => _.Action.WBSParts[WBSWebActionUtil<T>.IndentationFromWBS(_.Action.WBS)])
                    .ToList();
            }

            return wrappedAction;
        }

        private void ApplyRecursively(Node node)
        {
            for (var index = 0; index < node.Children.Count; index++)
            {
                var child = node.Children[index];
                var wbs = $"{node.Action.WBS}.{(index + 1).ToString()}";
                child.Action.WBS = wbs;
                if (!_list.Contains(child.Action))
                    _list.Add(child.Action);
                node.Children.ForEach(ApplyRecursively);
            }
        }


        /// <summary>
        /// Construit un arbre d'<see cref="Models.KAction"/> à partir d'une liste plate d'<see cref="Models.KAction"/>
        /// </summary>
        /// <param name="actions"></param>
        /// <returns></returns>
        public static WBSTreeVirtualizerUtil<T> VirtualizeTree(IEnumerable<T> actions)
        {
            return new WBSTreeVirtualizerUtil<T>(actions);
        }

        /// <summary>
        /// Supprime l'<see cref="Models.KAction"/> spécifiée au sein de l'arbre
        /// </summary>
        /// <param name="virtualizer"></param>
        /// <param name="action">L'<see cref="Models.KAction"/> à supprimer de l'arbre</param>
        /// <returns></returns>
        public static WBSTreeVirtualizerUtil<T> Remove(WBSTreeVirtualizerUtil<T> virtualizer, T action)
        {
            var node = virtualizer.GetNode(action);
            if (node == null)
            {
                throw new InvalidOperationException($"Impossible de résoudre l'action dans l'arbre du projet");
            }

            var parent = virtualizer.GetParentNode(action);
            if (parent == null)
            {
                if (!virtualizer.ActionTree.Contains(node))
                {
                    throw new InvalidOperationException($"Impossible de trouver un parent à l'action");
                }

                virtualizer.ActionTree.Remove(node);
            }
            else
            {
                parent.Children.Remove(node);
            }

            return virtualizer;
        }

        /// <summary>
        /// Représente une <see cref="Models.KAction"/> virtualisée sous forme de noeud.
        /// </summary>
        /// 

        [Serializable]
        public class Node //: ICloneable
        {

            public Node()
            {

            }

            public T Action { get; set; }

            //public TreeVirtualizedAction Parent { get; set; }

            public List<Node> Children
            {
                get;
                set;
            }



            public Node(T action)
            {
                this.Action = action;
                Children = new List<Node>();
            }


            //public object Clone()
            //{

            //    Node clone = (Node)MemberwiseClone();
            //    clone.Action = (T)this.Action.Clone();
            //    for (int i = 0; i < clone.Children.Count; i++)
            //    {
            //        CloneRecursively(clone, this);
            //    }

            //    return clone;
            //}

            //private void CloneRecursively(Node clonedNnode, Node toCloneNode)
            //{
            //    for (var i = 0; i < clonedNnode.Children.Count; i++)
            //    {
            //        clonedNnode.Children[i].Action = (T)toCloneNode.Children[i].Action.Clone();
            //        _list.Add(clonedNnode.Children[i].Action);
            //        CloneRecursively(clonedNnode.Children[i], toCloneNode.Children[i]);

            //    }
            //}
        }

    }

}