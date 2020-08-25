using System;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;

namespace KProcess.Data
{
    /// <summary>
    /// Contient des méthodes d'extensions pour l'ObjectContext d'Entity Framework.
    /// </summary>
    public static class ObjectContextExt
    {

        /// <summary>
        /// Définit une valeur pour une relation de type Reference (end en 0..1).
        /// Se fait grâce au nom de la relation.
        /// </summary>
        /// <typeparam name="TTargetEntity">Le type de la cible (0..1).</typeparam>
        /// <typeparam name="TSourceEntity">le type de la source (*).</typeparam>
        /// <param name="context">Le contexte.</param>
        /// <param name="sourceEntity">L'entité source.</param>
        /// <param name="targetEntity">L'entité cible.</param>
        /// <param name="relationshipName">Le nom de la relation. Doit être préfixé par le nom du contexte.</param>
        /// <param name="targetEndRoleName">Le nom du rôle qui représente la cible.</param>
        /// <returns><c>true</c> si l'opération a réussi.</returns>
        public static bool SetRelationShipReferenceValue<TTargetEntity, TSourceEntity>(this ObjectContext context, TSourceEntity sourceEntity, TTargetEntity targetEntity, string relationshipName, string targetEndRoleName)
            where TTargetEntity : class
            where TSourceEntity : class
        {
            RelationshipManager rel;
            if (context.ObjectStateManager.TryGetRelationshipManager(sourceEntity, out rel))
            {
                var refe = rel.GetRelatedReference<TTargetEntity>(relationshipName, targetEndRoleName);
                if (refe != null)
                {
                    refe.Value = targetEntity;
                    return true;
                }
            }

            return false;

        }

        /// <summary>
        /// Définit une valeur pour une relation de type Reference (end en 0..1).
        /// Se fait grâce à un accesseur à la clé étrangère correspondante.
        /// </summary>
        /// <typeparam name="TTargetEntity">Le type de la cible (0..1).</typeparam>
        /// <typeparam name="TSourceEntity">le type de la source (*).</typeparam>
        /// <typeparam name="TForeignKey">Le type de clé étrangère utilisée.</typeparam>
        /// <param name="context">Le contexte.</param>
        /// <param name="sourceEntity">L'entité source.</param>
        /// <param name="targetEntity">L'entité cible.</param>
        /// <param name="foreignKeyGetter">Un délégué capable d'obtenir la propriété représentant la clé étrangère. Attention, cet accesseur doit renvoyer directement une propriété, sans aucune opération supplémentaire.</param>
        /// <returns><c>true</c> si l'opération a réussi.</returns>
        public static bool SetRelationShipReferenceValue<TTargetEntity, TSourceEntity, TForeignKey>(
            this ObjectContext context,
            TSourceEntity sourceEntity,
            TTargetEntity targetEntity,
            Expression<Func<TSourceEntity, TForeignKey>> foreignKeyGetter)
            where TTargetEntity : class
            where TSourceEntity : class
        {
            var metadataWorkspace = context.MetadataWorkspace;
            ItemCollection cCollection;
            if (metadataWorkspace.TryGetItemCollection(DataSpace.CSpace, out cCollection))
            {
                var matchingEntityType = cCollection.GetItems<EntityType>().First(e => e.Name == typeof(TSourceEntity).Name);

                if (matchingEntityType != null)
                {
                    var foreignKeyPropertyName = ReflectionHelper.GetExpressionPropertyName(foreignKeyGetter);

                    var edmProperty = matchingEntityType.Properties.Where(p => p.TypeUsage.EdmType is PrimitiveType && p.Name == foreignKeyPropertyName).FirstOrDefault();

                    if (edmProperty != null)
                    {
                        string associationName = null;
                        string targetRoleName = null;

                        foreach (var association in cCollection.GetItems<AssociationType>())
                        {
                            foreach (var rc in association.ReferentialConstraints)
                            {
                                if (Enumerable.SequenceEqual(rc.ToProperties, new EdmProperty[] { edmProperty }))
                                {
                                    associationName = association.FullName;
                                    targetRoleName = rc.FromRole.Name;
                                    break;
                                }
                            }

                            if (associationName != null)
                                break;
                        }

                        if (associationName != null)
                        {
                            SetRelationShipReferenceValue(context, sourceEntity, targetEntity,
                                associationName, targetRoleName);
                            return true;
                        }

                    }
                }
            }

            return false;
        }
    }
}
