// -----------------------------------------------------------------------
// <copyright file="TransientCompositionContainer.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Conteneur permettant de fournir une nouvelle instance à chaque appel lorsque une metadata 
    /// </summary>
    public class TransientCompositionContainer : System.ComponentModel.Composition.Hosting.CompositionContainer
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="TransientCompositionContainer"/>.
        /// </summary>
        /// <param name="catalog">Un catalogue fournissant des objets <see cref="System.ComponentModel.Composition.Primitives.Export"/> au conteneur.</param>
        /// <param name="providers">Un tableur de fournisseur d'exports.</param>
        public TransientCompositionContainer(ComposablePartCatalog catalog, params ExportProvider[] providers)
            : base(catalog, providers)
        {
        }

        /// <summary>
        /// Retourne une collection de tous les exports qui correspondent aux conditions spécifiées dans l'<see cref="T:System.ComponentModel.Composition.Primitives.ImportDefinition"/>.
        /// </summary>
        /// <param name="definition">un objet définissant les conditions d'export</param>
        /// <param name="atomicComposition">La transaction de composition à utiliser.</param>
        /// <returns>
        /// Une collection des exports dans ce conteneur correspondant aux conditions de la définition.
        /// </returns>
        protected override IEnumerable<System.ComponentModel.Composition.Primitives.Export> GetExportsCore(System.ComponentModel.Composition.Primitives.ImportDefinition definition, AtomicComposition atomicComposition)
        {
            return base.GetExportsCore(AdaptDefinition(definition), atomicComposition);
        }

        /// <summary>
        /// Adapte la définition en fonction de l'export.
        /// </summary>
        /// <param name="definition">La defintion de l'import.</param>
        /// <returns>Une défintion adaptée.</returns>
        private ImportDefinition AdaptDefinition(ImportDefinition definition)
        {
            ContractBasedImportDefinition namedDefinition = definition as ContractBasedImportDefinition;
            if (namedDefinition != null && namedDefinition.RequiredCreationPolicy == CreationPolicy.Any && namedDefinition.RequiredMetadata != null && namedDefinition.RequiredMetadata.Any(kv => kv.Key == "LifetimeManagement"))
            {
                definition = new ContractBasedImportDefinition( namedDefinition.ContractName,
                                                                namedDefinition.RequiredTypeIdentity,
                                                                namedDefinition.RequiredMetadata,
                                                                namedDefinition.Cardinality,
                                                                namedDefinition.IsRecomposable,
                                                                namedDefinition.IsPrerequisite,
                                                                CreationPolicy.NonShared);
            }

            return definition;
        }        
    }
}