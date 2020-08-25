using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Représente un catalogue qui filtre les assemblies chargées par rapport à leur nom fort.
    /// </summary>
    public class StrongNameCatalog : ComposablePartCatalog
    {
        AggregateCatalog _aggregateCatalog = new AggregateCatalog();

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="StrongNameCatalog"/>.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="searchPattern">The file search pattern.</param>
        /// <param name="isRecursive"><c>true</c> to serach recursively.</param>
        /// <param name="publicKeyTokens">The trusted keys.</param>
        public StrongNameCatalog(string path, string searchPattern, bool isRecursive, params string[] publicKeyTokens)
        {
            var keyTokensBytes = publicKeyTokens.Select(s => s.ToByteArray()).ToArray();

            if (Directory.Exists(path))
                foreach (var file in Directory.GetFiles(path, searchPattern, isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                {
                    AssemblyName assemblyName = null;
                    try
                    {
                        assemblyName = AssemblyName.GetAssemblyName(file);
                    }
                    catch (ArgumentException)
                    {
                        //  According to MSDN, ArgumentException can be thrown
                        //  if the assembly file is invalid
                    }
                    catch (BadImageFormatException)
                    {
                        //  Not a valid assembly
                    }

                    if (assemblyName != null)
                    {
                        var publicKey = assemblyName.GetPublicKey();
                        if (publicKey != null)
                        {
                            bool trusted = false;
                            foreach (var publicKeyToken in keyTokensBytes)
                            {
                                if (assemblyName.GetPublicKeyToken().SequenceEqual(publicKeyToken))
                                {
                                    trusted = true;
                                    break;
                                }
                            }

                            if (trusted)
                                _aggregateCatalog.Catalogs.Add(new AssemblyCatalog(file));
                        }
                    }
                }
        }

        /// <inheritdoc />
        public override IQueryable<ComposablePartDefinition> Parts =>
            _aggregateCatalog.Parts;

        /// <inheritdoc />
        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition) =>
            _aggregateCatalog.GetExports(definition);
    }


}
