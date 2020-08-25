using DlhSoft.Windows.Controls;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.ViewModels;
using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace KProcess.Ksmed.Presentation.Shell.ViewModels
{
    partial class ActionsGraphManipulator<TActionItem>
    {
        /// <summary>
        /// Contient la référence d'une action feuille dans le graph des actions
        /// </summary>
        protected sealed class LeafNode : Node
        {
            public LeafNode(TActionItem action, INodeContainer parent)
                : base(action, parent)
            {

            }
        }

        /// <summary>
        /// Correspond au noeud racine de l'arbre
        /// </summary>
        protected sealed class RootNode : INodeContainer
        {
            public RootNode(ActionsGraphManipulator<TActionItem> owner)
            {
                this.Items = new List<Node>();
                this.Owner = owner;
            }

            /// <inheritDoc />
            public IList<Node> Items { get; private set; }

            /// <inheritDoc />
            public INodeContainer Parent { get { return null; } }

            /// <inheritDoc />
            public bool IsRoot
            {
                get { return true; }
            }

            public ActionsGraphManipulator<TActionItem> Owner { get; private set; }
        }

        /// <summary>
        /// Contient la référence d'une action de type groupe dans le graph des actions
        /// </summary>
        protected sealed class GroupNode : Node, INodeContainer
        {
            public GroupNode(TActionItem action, INodeContainer parent)
                : base(action, parent)
            {
                this.Items = new List<Node>();
            }

            /// <inheritDoc />
            public IList<Node> Items { get; private set; }

            /// <inheritDoc />
            public bool IsRoot { get { return false; } }

            /// <inheritDoc />
            public ActionsGraphManipulator<TActionItem> Owner
            {
                get { throw new NotImplementedException(); }
            }
        }

        /// <summary>
        /// Contient l'abstraction d'un noeud du graph d'actions
        /// </summary>
        public abstract class Node
        {
            protected Node(TActionItem action, INodeContainer parent)
            {
                this.Content = action;
                this.Parent = parent;
            }

            /// <summary>
            /// Obtient l'element de grille d'une action
            /// </summary>
            public TActionItem Content { get; private set; }

            /// <summary>
            /// Obtient le noeud parent s'il existe
            /// </summary>
            public INodeContainer Parent { get; protected set; }

            private ICommand _moveUpCommand = null;
            private ICommand _moveDownCommand = null;

            /// <summary>
            /// Obtient la command pour déplacer le noeud courant vers le haut
            /// </summary>
            public ICommand MoveUpCommand { get { return _moveUpCommand = _moveUpCommand ?? new Command(MoveUp, CanMoveUp); } }

            /// <summary>
            /// Obtient la command pour déplacer le noeud courant vers le bas
            /// </summary>
            public ICommand MoveDownCommand { get { return _moveDownCommand = _moveDownCommand ?? new Command(MoveDown, CanMoveDown); } }

            /// <summary>
            /// Calcule la position de l'item courant au sein de son conteneur courant
            /// </summary>
            /// <returns></returns>
            public int GetIndex()
            {
                return this.Parent.Items.IndexOf(this);
            }

            /// <summary>
            /// Determine si le noeud courant est premier dans son conteneur
            /// </summary>
            /// <returns></returns>
            public bool IsFirst()
            {
                return this.GetIndex() == 0;
            }

            /// <summary>
            /// Determine si le noeud courant est premier dans son conteneur
            /// </summary>
            /// <returns></returns>
            public bool IsLast()
            {
                return this == this.Parent.Items.Last();
            }

            /// <summary>
            /// Calcule le niveau de l'element courant vis à vis de son niveau de parenté par rapport au graph
            /// </summary>
            /// <returns></returns>
            public int GetLevel()
            {
                var level = 0;
                for (var parent = this.Parent; !parent.IsRoot; parent = parent.Parent)
                {
                    level++;
                }

                return level;
            }

            /// <summary>
            /// Parcourt lles liens de parenté du noeud courant jusqu'à trouver le possesseur du graph courant.
            /// </summary>
            /// <returns></returns>
            protected ActionsGraphManipulator<TActionItem> GetOwner()
            {
                var parent = this.Parent;
                while (!parent.IsRoot)
                {
                    parent = parent.Parent;
                }

                return ((RootNode)parent).Owner;
            }

            /// <summary>
            /// Détermine si l'item courant peut être déplacer vers le haut
            /// </summary>
            /// <returns></returns>
            private bool CanMoveUp()
            {
                return GetIndex() > 0 || !this.Parent.IsRoot && this.Parent.Items.Count > 1; // Le dernier test empeche un groupe d'être vide. Ce cas est géré mais non supporté dans le diagramme de gantt
            }

            /// <summary>
            /// Effectue une monté de l'élement courant vers le haut si c'est possible
            /// </summary>
            private void MoveUp()
            {
                if(CanMoveUp())
                {
                    var index = GetIndex();
                    
                    if(this.IsFirst()) // Sort du groupe
                    {
                        var currentContainer = this.Parent;
                        var newContainer = currentContainer.Parent;
                        var newIndex = newContainer.Items.IndexOf((Node)currentContainer);

                        currentContainer.Items.Remove(this);
                        newContainer.Items.Insert(newIndex, this);
                        this.Parent = newContainer; // Attention, synchronisation manuelle du lien de parenté.

                        if(!currentContainer.IsRoot && !currentContainer.Items.Any())
                        {
                            currentContainer.Parent.Items.Remove((Node)currentContainer); // On supprime les conteneurs vides
                        }
                    }
                    else if (this.Parent.Items[index - 1] is INodeContainer) // Intégre la fin du groupe du dessus
                    {
                        var currentContainer = this.Parent;
                        var newContainer = (INodeContainer)this.Parent.Items[index - 1];

                        currentContainer.Items.Remove(this);
                        newContainer.Items.Add(this);
                        this.Parent = newContainer; // Attention, synchronisation manuelle du lien de parenté.
                    }
                    else // Remonte au sein de son propre groupe
                    {
                        var container = this.Parent;
                        var newIndex = index - 1;

                        container.Items.Remove(this);
                        container.Items.Insert(newIndex, this);
                    }
                }
            }

            /// <summary>
            /// Détermine si l'item courant peut être déplacer vers le haut
            /// </summary>
            /// <returns></returns>
            private bool CanMoveDown()
            {
                return this.GetIndex() < this.Parent.Items.Count - 1 || !this.Parent.IsRoot && this.Parent.Items.Count > 1; // Le dernier test empeche un groupe d'être vide. Ce cas est géré mais non supporté dans le diagramme de gantt
            }

            /// <summary>
            /// Effectue une monté de l'élement courant vers le bas si c'est possible
            /// </summary>
            private void MoveDown()
            {
                if (CanMoveDown())
                {
                    var index = GetIndex();

                    if (this.IsLast()) // Sort du groupe
                    {
                        var currentContainer = this.Parent;
                        var newContainer = currentContainer.Parent;
                        var newIndex = newContainer.Items.IndexOf((Node)currentContainer) + 1;

                        currentContainer.Items.Remove(this);
                        if (newIndex == newContainer.Items.Count)
                        {
                            newContainer.Items.Add(this);
                        }
                        else
                        {
                            newContainer.Items.Insert(newIndex, this);
                        }

                        this.Parent = newContainer; // Attention, synchronisation manuelle du lien de parenté.

                        if (!currentContainer.IsRoot && !currentContainer.Items.Any())
                        {
                            currentContainer.Parent.Items.Remove((Node)currentContainer); // On supprime les conteneurs vides
                        }
                    }
                    else if (this.Parent.Items[index + 1] is INodeContainer) // Intégre le début du groupe du dessous
                    {
                        var currentContainer = this.Parent;
                        var newContainer = (INodeContainer)this.Parent.Items[index + 1];

                        currentContainer.Items.Remove(this);
                        if (!newContainer.Items.Any())
                        {
                            newContainer.Items.Add(this);
                        }
                        else
                        {
                            newContainer.Items.Insert(0, this);
                        }

                        this.Parent = newContainer; // Attention, synchronisation manuelle du lien de parenté.
                    }
                    else // Descend au sein de son propre groupe
                    {
                        var container = this.Parent;
                        var newIndex = index + 1;

                        container.Items.Remove(this);
                        container.Items.Insert(newIndex, this);
                    }
                }
            }
        }



        /// <summary>
        /// Décrit un noeud capable de contenir des noeuds enfants
        /// </summary>
        public interface INodeContainer
        {
            /// <summary>
            /// Contient les elements enfants directs du conteneur courant
            /// </summary>
            IList<Node> Items { get; }

            /// <summary>
            /// Obtient le noeud parent s'il existe
            /// </summary>
            /// <remarks>
            /// Tester la propriété IsRoot pour savoir si le noeud courant correspond à un element racine
            /// </remarks>
            INodeContainer Parent { get; }

            /// <summary>
            /// Détermine S'il s'agit d'un conteneur racine
            /// </summary>
            bool IsRoot { get; }

            /// <summary>
            /// Obient le possesseur du graph de l'element courant
            /// </summary>
            ActionsGraphManipulator<TActionItem> Owner { get; }
        }
    }
}
