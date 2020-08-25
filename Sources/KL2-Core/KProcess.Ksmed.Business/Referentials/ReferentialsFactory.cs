using KProcess.Ksmed.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KProcess.Ksmed.Business.Referentials
{
    public static class ReferentialsFactory
    {
        /// <summary>
        /// Crée une copie du référentiel spécifié, en créant un référentiel projet.
        /// </summary>
        /// <param name="referential">Le référentiel.</param>
        /// <returns>Le référentiel créé.</returns>
        public static IActionReferentialProcess CopyToNewProject(IActionReferential referential)
        {
            if (referential is ActionCategory)
                return CopyToNewProject<ActionCategory>(referential, nameof(ActionCategory.ActionCategoryId));
            if (referential is Equipment)
                return CopyToNewProject<Equipment>(referential, nameof(Resource.ResourceId));
            if (referential is Operator)
                return CopyToNewProject<Operator>(referential, nameof(Resource.ResourceId));
            if (referential is Ref1)
                return CopyToNewProject<Ref1>(referential, nameof(Ref1.RefId));
            if (referential is Ref2)
                return CopyToNewProject<Ref2>(referential, nameof(Ref2.RefId));
            if (referential is Ref3)
                return CopyToNewProject<Ref3>(referential, nameof(Ref3.RefId));
            if (referential is Ref4)
                return CopyToNewProject<Ref4>(referential, nameof(Ref4.RefId));
            if (referential is Ref5)
                return CopyToNewProject<Ref5>(referential, nameof(Ref5.RefId));
            if (referential is Ref6)
                return CopyToNewProject<Ref6>(referential, nameof(Ref6.RefId));
            if (referential is Ref7)
                return CopyToNewProject<Ref7>(referential, nameof(Ref7.RefId));

            throw new ArgumentException("Le référentiel est inconnu", nameof(referential));
        }

        static TProject CopyToNewProject<TProject>(IActionReferential referential, params string[] excludedProperties)
            where TProject : IActionReferentialProcess, IObjectWithChangeTracker, new()
        {
            TProject project = new TProject();

            IDictionary<string, object> originalValues = referential.GetCurrentValues();

            List<string> excludedPropertiesDefault = new List<string>
            {
                nameof(Resource.CreationDate),
                nameof(Resource.LastModificationDate),
                nameof(Resource.ModifiedByUserId),
                nameof(Resource.CreatedByUserId)
            };
            if ((referential as IActionReferentialProcess)?.ProcessId != null)
            {
                excludedPropertiesDefault.Add(nameof(IActionReferentialProcess.ProcessId));
                excludedPropertiesDefault.Add(nameof(IActionReferentialProcess.Process));
            }

            // Vérifier que ces noms de propriétés soient corrects
            if (excludedProperties.Concat(excludedPropertiesDefault).Except(originalValues.Keys).Any())
                throw new InvalidOperationException("Les noms de propriétés présents dans excludedProperties ne sont pas valides.");

            foreach (KeyValuePair<string, object> kvp in originalValues)
            {
                if (!excludedProperties.Concat(excludedPropertiesDefault).Contains(kvp.Key))
                    project.SetPropertyValue(kvp.Key, kvp.Value);
            }

            return project;
        }

        /// <summary>
        /// Clone un lien référentiel - action.
        /// </summary>
        /// <typeparam name="TReferentialActionLink">Le type du lien.</typeparam>
        /// <param name="oldAl">L'ancien lien.</param>
        /// <param name="copyReferential"><c>true</c> pour copier le lien vers le referentiel</param>
        /// <param name="copyAction"><c>true</c> pour copier le lien vers l'action</param>
        /// <returns>Le nouveau lien.</returns>
        public static TReferentialActionLink CloneReferentialActionsLink<TReferentialActionLink>(TReferentialActionLink oldAl, bool copyReferential, bool copyAction)
            where TReferentialActionLink : IReferentialActionLink, new()
        {
            TReferentialActionLink newAl = new TReferentialActionLink
            {
                Quantity = oldAl.Quantity
            };

            if (copyReferential)
            {
                newAl.ReferentialId = oldAl.ReferentialId;
                newAl.Referential = oldAl.Referential;
            }

            if (copyAction)
            {
                newAl.ActionId = oldAl.ActionId;
                newAl.Action = oldAl.Action;
            }
            return newAl;
        }

        /// <summary>
        /// Clone un lien référentiel - documentation action.
        /// </summary>
        /// <param name="oldAl">L'ancien lien.</param>
        /// <param name="copyAction"><c>true</c> pour copier le lien vers l'action</param>
        /// <returns>Le nouveau lien.</returns>
        public static ReferentialDocumentationActionDraft CloneReferentialDocumentationActionsLink(ReferentialDocumentationActionDraft oldAl, bool copyAction)
        {
            ReferentialDocumentationActionDraft newAl = new ReferentialDocumentationActionDraft
            {
                RefNumber = oldAl.RefNumber,
                Quantity = oldAl.Quantity
            };

            if (copyAction)
            {
                newAl.DocumentationActionDraftId = oldAl.DocumentationActionDraftId;
                newAl.DocumentationActionDraft = oldAl.DocumentationActionDraft;
            }
            return newAl;
        }

        /// <summary>
        /// Obtient un lien référentiel - action.
        /// </summary>
        /// <param name="referential">Le référentiel.</param>
        /// <param name="action">L'action</param>
        /// <returns>Le lien.</returns>
        public static IReferentialActionLink GetReferentialActionLink(IMultipleActionReferential referential, KAction action)
        {
            if (referential is Ref1)
                return action.Ref1.FirstOrDefault(al => al.Referential == referential);
            if (referential is Ref2)
                return action.Ref2.FirstOrDefault(al => al.Referential == referential);
            if (referential is Ref3)
                return action.Ref3.FirstOrDefault(al => al.Referential == referential);
            if (referential is Ref4)
                return action.Ref4.FirstOrDefault(al => al.Referential == referential);
            if (referential is Ref5)
                return action.Ref5.FirstOrDefault(al => al.Referential == referential);
            if (referential is Ref6)
                return action.Ref6.FirstOrDefault(al => al.Referential == referential);
            if (referential is Ref7)
                return action.Ref7.FirstOrDefault(al => al.Referential == referential);
            
            throw new ArgumentOutOfRangeException(nameof(referential));
        }

        /// <summary>
        /// Crée un lien référentiel - Action.
        /// </summary>
        /// <param name="referential">Le référentiel.</param>
        /// <param name="action">L'action</param>
        /// <param name="quantity">La quantité.</param>
        /// <returns>Le lien.</returns>
        public static IReferentialActionLink CreateReferentialActionLink(IMultipleActionReferential referential, KAction action, int quantity)
        {
            IReferentialActionLink al;
            IList list;

            if (referential is Ref1)
            {
                al = new Ref1Action();
                list = action.Ref1;
            }
            else if (referential is Ref2)
            {
                al = new Ref2Action();
                list = action.Ref2;
            }
            else if (referential is Ref3)
            {
                al = new Ref3Action();
                list = action.Ref3;
            }
            else if (referential is Ref4)
            {
                al = new Ref4Action();
                list = action.Ref4;
            }
            else if (referential is Ref5)
            {
                al = new Ref5Action();
                list = action.Ref5;
            }
            else if (referential is Ref6)
            {
                al = new Ref6Action();
                list = action.Ref6;
            }
            else if (referential is Ref7)
            {
                al = new Ref7Action();
                list = action.Ref7;
            }
            else
                throw new ArgumentOutOfRangeException(nameof(referential));

            al.Action = action;
            al.Referential = referential;
            al.Quantity = quantity;

            list.Add(al);

            return al;
        }

        /// <summary>
        /// Vide la collection de liens action - référentiel.
        /// </summary>
        /// <param name="referential">Le type de référentiel.</param>
        /// <param name="action">L'action.</param>
        public static void ClearReferentialActionLinks(ProcessReferentialIdentifier referential, KAction action, bool cancelChanges = false)
        {
            switch (referential)
            {
                case ProcessReferentialIdentifier.Ref1:
                    foreach (Ref1Action refe in action.Ref1.ToArray())
                        Delete(action.Ref1, refe.Referential, cancelChanges);
                    break;
                case ProcessReferentialIdentifier.Ref2:
                    foreach (Ref2Action refe in action.Ref2.ToArray())
                        Delete(action.Ref2, refe.Referential, cancelChanges);
                    break;
                case ProcessReferentialIdentifier.Ref3:
                    foreach (Ref3Action refe in action.Ref3.ToArray())
                        Delete(action.Ref3, refe.Referential, cancelChanges);
                    break;
                case ProcessReferentialIdentifier.Ref4:
                    foreach (Ref4Action refe in action.Ref4.ToArray())
                        Delete(action.Ref4, refe.Referential, cancelChanges);
                    break;
                case ProcessReferentialIdentifier.Ref5:
                    foreach (Ref5Action refe in action.Ref5.ToArray())
                        Delete(action.Ref5, refe.Referential, cancelChanges);
                    break;
                case ProcessReferentialIdentifier.Ref6:
                    foreach (Ref6Action refe in action.Ref6.ToArray())
                        Delete(action.Ref6, refe.Referential, cancelChanges);
                    break;
                case ProcessReferentialIdentifier.Ref7:
                    foreach (Ref7Action refe in action.Ref7.ToArray())
                        Delete(action.Ref7, refe.Referential, cancelChanges);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(referential));
            }
        }

        /// <summary>
        /// Supprime un lien référentiel - Action.
        /// </summary>
        /// <param name="referential">Le référentiel.</param>
        /// <param name="action">L'action.</param>
        /// <returns><c>true</c> si la suppression a réussi.</returns>
        public static bool DeleteReferentialActionLink(IMultipleActionReferential referential, KAction action)
        {
            if (referential is Ref1)
                return Delete(action.Ref1, referential);
            if (referential is Ref2)
                return Delete(action.Ref2, referential);
            if (referential is Ref3)
                return Delete(action.Ref3, referential);
            if (referential is Ref4)
                return Delete(action.Ref4, referential);
            if (referential is Ref5)
                return Delete(action.Ref5, referential);
            if (referential is Ref6)
                return Delete(action.Ref6, referential);
            if (referential is Ref7)
                return Delete(action.Ref7, referential);
            
            throw new ArgumentOutOfRangeException(nameof(referential));
        }

        /// <summary>
        /// Supprime un lien référentiel - Action.
        /// </summary>
        /// <typeparam name="TActionLink">Le type de lien référentiel - action.</typeparam>
        /// <param name="collection">La collection dans laquelle l'élément doit être supprimé.</param>
        /// <param name="referential">Le référentiel.</param>
        /// <returns><c>true</c> si la suppression a réussi.</returns>
        static bool Delete<TActionLink>(TrackableCollection<TActionLink> collection, IMultipleActionReferential referential, bool cancelChanges = false)
            where TActionLink : IReferentialActionLink, IObjectWithChangeTracker
        {
            TActionLink al = collection.FirstOrDefault(c => c.Referential == referential);
            if (al != null)
            {
                if (cancelChanges)
                    al.Referential.CancelChanges();
                collection.Remove(al);
                if (al.IsMarkedAsAdded)
                {
                    al.MarkAsUnchanged();
                }

                return true;
            }
            return false;
        }

    }
}
