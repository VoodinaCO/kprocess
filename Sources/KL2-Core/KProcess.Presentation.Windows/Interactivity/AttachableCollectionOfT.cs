using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interactivity;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Représente une collection d'IAttachedObject qui fournit les notifications de changement.
    /// </summary>
    /// <typeparam name="T">Le type d'objets contenu.</typeparam>
    public abstract class AttachableCollection<T> : FreezableCollection<T>, IAttachedObject
        where T : DependencyObject, IAttachedObject
    {
        #region Champs

        private DependencyObject _associatedObject;
        /// <summary>
        /// Obtient ou définit le snapshot.
        /// </summary>
        protected Collection<T> Snapshot { get; set; }

        #endregion

        #region Constructeurs

        /// <summary>
        /// Constructeur
        /// </summary>
        internal AttachableCollection()
            : this(new Collection<T>())
        {
        }

        /// <summary>
        /// Constructeur.
        /// </summary>
        /// <param name="snapshot">La collection contenant les objets actuels.</param>
        internal AttachableCollection(Collection<T> snapshot)
        {
            INotifyCollectionChanged changed = this;
            changed.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
            this.Snapshot = snapshot;
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Attache la collection à l'objet spécifié.
        /// </summary>
        /// <param name="dependencyObject">L'objet auquel s'attacher.</param>
        public void Attach(DependencyObject dependencyObject)
        {
            if (dependencyObject != this.AssociatedObject)
            {
                if (this.AssociatedObject != null)
                {
                    throw new InvalidOperationException();
                }
                if (!DesignMode.IsInDesignMode)
                {
                    base.WritePreamble();
                    this._associatedObject = dependencyObject;
                    base.WritePostscript();
                }
                this.OnAttached();
            }
        }

        /// <summary>
        /// Detache la collection de son objet associé.
        /// </summary>
        public void Detach()
        {
            this.OnDetaching();
            base.WritePreamble();
            this._associatedObject = null;
            base.WritePostscript();
        }

        #endregion

        #region Méthodes protégées

        /// <summary>
        /// Appelé lorsqu'un objet est ajouté à la collection.
        /// </summary>
        /// <param name="item">L'objet.</param>
        protected abstract void ItemAdded(T item);

        /// <summary>
        /// Appelé lorsqu'un objet est supprimé de la collection.
        /// </summary>
        /// <param name="item">L'objet.</param>
        protected abstract void ItemRemoved(T item);

        /// <summary>
        /// Appelé après que la collection soit attachée avec l'<see cref="AssociatedObject"/>.
        /// </summary>
        protected abstract void OnAttached();

        /// <summary>
        /// Appelé avant que la collection soit détachée de l'<see cref="AssociatedObject"/>.
        /// </summary>
        protected abstract void OnDetaching();

        /// <summary>
        /// Rend l'objet Freezable non modifiable ou vérifie si celui-ci peut être rendu non modifiable ou pas.
        /// </summary>
        /// <param name="isChecking">true pour retourner une indication de la possibilité ou non de figer l'objet (sans le figer réellement) ; false pour figer réellement l'objet.</param>
        /// <returns>Si isChecking est true, cette méthode retourne true si le Freezable peut être rendu non modifiable, ou false si cette opération est impossible.Si isChecking est false, cette méthode retourne true si le Freezable spécifié est désormais non modifiable, ou false si cette opération est impossible.</returns>
        protected override bool FreezeCore(bool isChecking)
        {
            return false;
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Appelé lorsque la collection a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les données de l'évènement.</param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (T local in e.NewItems)
                    {
                        try
                        {
                            this.VerifyAdd(local);
                            this.ItemAdded(local);
                            continue;
                        }
                        finally
                        {
                            this.Snapshot.Insert(base.IndexOf(local), local);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (T local4 in e.OldItems)
                    {
                        this.ItemRemoved(local4);
                        this.Snapshot.Remove(local4);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (T local2 in e.OldItems)
                    {
                        this.ItemRemoved(local2);
                        this.Snapshot.Remove(local2);
                    }
                    foreach (T local3 in e.NewItems)
                    {
                        try
                        {
                            this.VerifyAdd(local3);
                            this.ItemAdded(local3);
                            continue;
                        }
                        finally
                        {
                            this.Snapshot.Insert(base.IndexOf(local3), local3);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    foreach (T local5 in this.Snapshot)
                    {
                        this.ItemRemoved(local5);
                    }
                    this.Snapshot = new Collection<T>();
                    foreach (T local6 in this)
                    {
                        this.VerifyAdd(local6);
                        this.ItemAdded(local6);
                    }
                    break;

                default:
                    return;
            }
        }

        /// <summary>
        /// Vérifie qu'un object puisse être ajouté.
        /// </summary>
        /// <param name="item">L'objet.</param>
        private void VerifyAdd(T item)
        {
            if (this.Snapshot.Contains(item))
            {
                throw new InvalidOperationException("Impossible d'ajouter des éléments en double");
            }
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient l'objet associé.
        /// </summary>
        public DependencyObject AssociatedObject
        {
            get
            {
                base.ReadPreamble();
                return this._associatedObject;
            }
        }

        #endregion
    }
}
