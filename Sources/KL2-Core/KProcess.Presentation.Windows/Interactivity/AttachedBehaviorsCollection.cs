using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interactivity;

namespace KProcess.Presentation.Windows
{

    /// <summary>
    /// Représente une collection de Behaviors dont le but est d'être attachés à un style.
    /// </summary>
    public class AttachedBehaviorsCollection : AttachableCollection<Behavior>
    {
        #region Champs

        private Dictionary<Behavior, Behavior> _clones;
        private bool _isAttached;

        #endregion

        #region Constructeurs

        /// <summary>
        /// Constructeur.
        /// </summary>
        public AttachedBehaviorsCollection()
            : this(new Collection<Behavior>())
        {
        }

        /// <summary>
        /// Constructeur.
        /// </summary>
        /// <param name="snapshot">La collection contenant les objets actuels.</param>
        private AttachedBehaviorsCollection(Collection<Behavior> snapshot)
            : base(snapshot)
        {
            _clones = new Dictionary<Behavior, Behavior>();
        }

        #endregion

        #region Surcharges

        /// <summary>
        /// Crée une instance de cette classe.
        /// </summary>
        /// <returns>Une instance de cette classe.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new AttachedBehaviorsCollection(base.Snapshot);
        }

        /// <summary>
        /// Appelé lorsqu'un objet est ajouté à la collection.
        /// </summary>
        /// <param name="item">L'objet.</param>
        protected override void ItemAdded(Behavior item)
        {
            if (_isAttached)
                throw new InvalidOperationException("La collection de behaviors ne peut être modifiée après avoir été attachée. Utiliser Interaction.GetBehaviors() à la place.");
        }

        /// <summary>
        /// Appelé lorsqu'un objet est supprimé de la collection.
        /// </summary>
        /// <param name="item">L'objet.</param>
        protected override void ItemRemoved(Behavior item)
        {
        }

        /// <summary>
        /// Appelé après que la collection soit attachée avec l'AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            if (this.AssociatedObject != null)
            {
                foreach (Behavior behavior in this)
                {
                    var clone = (Behavior)behavior.Clone();
                    if (clone is ICloneableBehavior)
                        ((ICloneableBehavior)clone).IsClone = true;
                    _clones[behavior] = clone;

                    Interaction.GetBehaviors(this.AssociatedObject).Add(clone);
                }

                _isAttached = true;
            }
        }

        /// <summary>
        /// Appelé avant que la collection soit détachée de l'AssociatedObject.
        /// </summary>
        protected override void OnDetaching()
        {
            if (this.AssociatedObject != null)
            {
                foreach (Behavior behavior in this)
                {
                    if (_clones.ContainsKey(behavior))
                    {
                        var clone = _clones[behavior];

                        Interaction.GetBehaviors(this.AssociatedObject).Remove(clone);

                        _clones.Remove(behavior);
                    }
                }
            }
        }

        #endregion
    }
}
