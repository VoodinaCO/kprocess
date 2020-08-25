using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace KProcess.Ksmed.Business.ActionsManagement
{
    /// <summary>
    /// Permet d'abstraire une liste de <see cref="Models.KAction"/> en arbre en se basant sur les WBS.
    /// </summary>
    ///
    [Serializable]
    public class WBSTreeVirtualizer
    {
        private readonly List<Node> _actionTree;
        public static List<KAction> _list = new List<KAction>();

        public List<Node> ActionTree
        {
            get
            {
                return _actionTree;
            }
        }

        public WBSTreeVirtualizer(IEnumerable<KAction> actionList)
        {
            // _list.AddRange(actionList);
            var splittedActions = actionList.Select(_ => new { splittedWBS = WBSHelper.GetParts(_.WBS), action = _ }).ToArray();

            var treeRoot = splittedActions.Where(splittedAction => splittedAction.splittedWBS.Length == 1)
                .Select(splittedAction => CreateRecursively(splittedAction.action, actionList))
                .OrderBy(_ => _.Action.WBSParts[0])
                .ToList();

            _actionTree = treeRoot;
        }

        public IEnumerable<KAction> ApplyWBS()
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

        public Node GetNode(KAction action) => this.GetAllNodes().FirstOrDefault(_ => _.Action == action);

        public Node GetParentNode(KAction action) => this.GetAllNodes().FirstOrDefault(node => node.Children.Any(_ => _.Action == action));

        public static IEnumerable<Node> GetDescendItems(Node node) => node.Children.Union(node.Children.SelectMany(GetDescendItems));

        private IEnumerable<Node> GetAllNodes()  => this.ActionTree.Union(this.ActionTree.SelectMany(GetDescendItems));

        public void AddNode(Node node)
        {
            _actionTree.Add(node);
        }

private Node CreateRecursively(KAction action, IEnumerable<KAction> actions)
        {
            var wrappedAction = new Node(action);
            wrappedAction.Children = new List<Node>();
            if (WBSHelper.HasChildren(action, actions))
            {
                wrappedAction.Children = WBSHelper.GetChildren(action, actions)
                    .Select(_ => CreateRecursively(_, actions))
                    .OrderBy(_ => _.Action.WBSParts[WBSHelper.IndentationFromWBS(_.Action.WBS)])
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
        /// Représente une <see cref="Models.KAction"/> virtualisée sous forme de noeud.
        /// </summary>
        /// 

        [Serializable]
        public class Node : ICloneable
        {

            public Node()
            {

            }

            public KAction Action { get; set; }

            //public TreeVirtualizedAction Parent { get; set; }

            public List<Node> Children
            {
                get;
                set;
            }

          

            public Node(KAction action)
            {
                this.Action = action;
                Children = new List<Node>();
            }

            public object Clone()
            {

                Node clone = (Node)MemberwiseClone();
                clone.Action = (KAction)this.Action.Clone();
                for (int i = 0; i < clone.Children.Count; i++)
                {
                    CloneRecursively(clone, this);
                }

                return clone;
            }

            private void CloneRecursively(Node clonedNnode, Node toCloneNode)
            {
                for (var i = 0; i < clonedNnode.Children.Count; i++)
                {
                    clonedNnode.Children[i].Action = (KAction)toCloneNode.Children[i].Action.Clone();
                    _list.Add(clonedNnode.Children[i].Action);
                    CloneRecursively(clonedNnode.Children[i], toCloneNode.Children[i]);
                   
                }
            }
        }

    }
}
