using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KProcess.KL2.Business.Impl.Shared.ProjectImportMigration
{
    /// <summary>
    /// Fournit des méthodes permettant d'effectuer des opérations de sérialisation.
    /// </summary>
    internal static class SerializationOperations
    {
        /// <summary>
        /// Le namespace pour les anciens "ProjectExport".
        /// </summary>
        private const string ProjectExportVersion = "KProcess.Ksmed.Business.Dtos.Export.OlderVersions.v{0}{1}{2}{3}";

        /// <summary>
        /// Le namespace pour les anciens models.
        /// </summary>
        private const string ModelsVersion = "KProcess.Ksmed.Models.OlderVersions.v{0}{1}{2}{3}";

        /// <summary>
        /// Désérialise un stream.
        /// </summary>
        /// <typeparam name="T">Le type de donnée contenu dans le stream.</typeparam>
        /// <param name="stream">Le stream contenant les données.</param>
        /// <returns>L'objet déserialisé.</returns>
        public static T Deserialize<T>(Stream stream) =>
            (T)Deserialize(stream, new AnyVersionSerializationBinder());

        /// <summary>
        /// Désérialise un stream en migrant les namespaces.
        /// </summary>
        /// <typeparam name="T">Le type de donnée contenu dans le stream.</typeparam>
        /// <param name="stream">Le stream contenant les données.</param>
        /// <param name="version">La version d'entrée.</param>
        /// <param name="fromOldVersionToNew"><c>true</c> si la désérialisation cible une ancienne version vers une nouvelle.</param>
        /// <returns>L'objet déserialisé.</returns>
        public static T DeserializeWithNamespacesChange<T>(Stream stream, Version version, bool fromOldVersionToNew) =>
            (T)Deserialize(stream, new SpecificNamespacesSerializationBinder(
                fromOldVersionToNew ? null : string.Format(ProjectExportVersion, version.Major, version.Minor, version.Build, version.Revision),
                fromOldVersionToNew ? null : string.Format(ModelsVersion, version.Major, version.Minor, version.Build, version.Revision)));

        /// <summary>
        /// Désérialise un stream.
        /// </summary>
        /// <param name="stream">Le stream contenant les données.</param>
        /// <param name="binder">Le lieur de types.</param>
        /// <returns>L'objet déserialisé.</returns>
        public static object Deserialize(Stream stream, SerializationBinder binder)
        {
            var rdr = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);
            var ser = new NetDataContractSerializer()
            {
                Binder = binder,
            };

            return ser.ReadObject(rdr);
        }

        /// <summary>
        /// Sérialise un objet.
        /// </summary>
        /// <param name="obj">L'objet.</param>
        /// <returns>Le stream contenant les données sérialisées.</returns>
        public static async Task<MemoryStream> Serialize(object obj)
        {
            var ms = new MemoryStream();

            var writer = XmlDictionaryWriter.CreateTextWriter(ms, Encoding.UTF8, false);

            var ser = new NetDataContractSerializer();
            ser.WriteObject(writer, obj);

            await writer.FlushAsync();

            return ms;
        }


        /// <summary>
        /// Représente un lieur de sérialisation qui ne prend pas en compte la version de l'assembly pour le chargement des types.
        /// </summary>
        internal class AnyVersionSerializationBinder : SerializationBinder
        {
            /// <inheritdoc />
            public override Type BindToType(string assemblyName, string typeName)
            {
                System.Reflection.AssemblyName an = new System.Reflection.AssemblyName(assemblyName)
                {
                    Version = null
                };
                return System.Reflection.Assembly.Load(an.ToString()).GetType(typeName);
            }
        }

        /// <summary>
        /// Représente un lieur de sérialisation qui migre les espaces de noms..
        /// </summary>
        internal class SpecificNamespacesSerializationBinder : SerializationBinder
        {
            // Interne pour les TUs
            internal static string OlderProjectExportNamespacePrefix = "KProcess.Ksmed.Business.Dtos.Export.OlderVersions.v";
            internal static string OlderModelsNamespace = "KProcess.Ksmed.Models.OlderVersions.v";

            internal static string StandardProjectExportNamespace = "KProcess.Ksmed.Business.Dtos.Export";
            internal static string StandardModelsNamespace = "KProcess.Ksmed.Models";

            private string _projectExportNamespace;
            private string _modelsNamespace;

            public SpecificNamespacesSerializationBinder()
            {
            }

            public SpecificNamespacesSerializationBinder(string projectExportNamespace, string modelsNamespace)
            {
                _projectExportNamespace = projectExportNamespace;
                _modelsNamespace = modelsNamespace;
            }

            internal static Func<string, Type> AssemblyTypeResolverOverride { get; set; }

            /// <inheritdoc />
            public override Type BindToType(string assemblyName, string typeName)
            {
                System.Reflection.AssemblyName an = new System.Reflection.AssemblyName(assemblyName)
                {
                    Version = null
                };

                var finalTypeName = typeName;

                var parts = typeName.Split(Type.Delimiter);

                if (typeName.StartsWith(OlderProjectExportNamespacePrefix))
                {
                    finalTypeName = string.Concat(StandardProjectExportNamespace, ".", parts[parts.Length - 1]);
                }
                else if (typeName.StartsWith(OlderModelsNamespace))
                {
                    finalTypeName = string.Concat(StandardModelsNamespace, ".", parts[parts.Length - 1]);
                }
                else if (typeName.StartsWith(StandardProjectExportNamespace))
                {
                    if (_projectExportNamespace != null)
                        finalTypeName = typeName.Replace(StandardProjectExportNamespace, _projectExportNamespace);
                }
                else if (typeName.StartsWith(StandardModelsNamespace))
                {
                    if (_modelsNamespace != null)
                        finalTypeName = typeName.Replace(StandardModelsNamespace, _modelsNamespace);
                }

                Type type = null;
                if (AssemblyTypeResolverOverride != null)
                    type = AssemblyTypeResolverOverride(finalTypeName);

                if (type == null)
                    type = System.Reflection.Assembly.Load(an).GetType(finalTypeName);

                if (type == null)
                    throw new InvalidOperationException(string.Format("Type non trouvé : {0}, {1}", assemblyName, typeName));

                return type;
            }

        }

    }
}