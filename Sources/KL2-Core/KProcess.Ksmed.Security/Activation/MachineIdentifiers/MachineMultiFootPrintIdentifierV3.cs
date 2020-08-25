using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Security.Activation.MachineIdentifiers
{
    /// <summary>
    /// Fournit un hash regroupant plusieurs numéros de série d'éléments matériels ainsi qu'un Guid logiciel généré.
    /// </summary>
    class MachineMultiFootPrintIdentifierV3 : MachineIdentifierBase, IMachineIdentifier
    {
        public MachineMultiFootPrintIdentifierV3(ITraceManager traceManager)
            : base(traceManager)
        {
        }

        protected override byte[] GetIdentifierHash()
        {
            var dockerIsUsed = DockerHelper.IsInContainer();

            _traceManager?.TraceDebug("MachineMultiFootPrintIdentifierV3.GetIdentifierHash");

            var mbSerialNumber = dockerIsUsed ? "docker" : MachineMultiFootPrintIdentifier.GetMBSerialNumber();
            _traceManager?.TraceDebug("MachineMultiFootPrintIdentifierV3.GetIdentifierHash : MBSerialNumber {0}", mbSerialNumber ?? string.Empty);

            var cpuID = dockerIsUsed ? "docker" : MachineMultiFootPrintIdentifier.GetCPUSerialNumber();
            _traceManager?.TraceDebug("MachineMultiFootPrintIdentifierV3.GetIdentifierHash : CPUSerialNumber {0}", cpuID ?? string.Empty);

            var guid = GetGuid();
            _traceManager?.TraceDebug("MachineMultiFootPrintIdentifierV3.GetIdentifierHash : Guid {0}", guid);

            var concat = string.Join("$",
                mbSerialNumber,
                cpuID,
                guid.ToString(null, CultureInfo.InvariantCulture)
                )
                .Trim();

            _traceManager?.TraceDebug("MachineMultiFootPrintIdentifierV3.GetIdentifierHash : Concaténé {0}", concat ?? string.Empty);

            if (concat != null)
                return ComputeHash(concat);
            else
                return null;
        }

        private const string GuidFileName = "MachineIdentifierGuid";

        private static readonly object _syncRoot = new object();

        /// <summary>
        /// Obtient le Guid stocké dans l'isolated storage.
        /// </summary>
        /// <returns>Le Guid.</returns>
        private Guid GetGuid()
        {
            _traceManager?.TraceDebug("MachineMultiFootPrintIdentifierV3.GetIdentifierHash : Récupération Guid");

            var machineStore = IsolatedStorageFile.GetMachineStoreForAssembly();
            if (machineStore != null)
            {
                if (machineStore.FileExists(GuidFileName))
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(GuidFileName, System.IO.FileMode.Open, FileAccess.Read, FileShare.ReadWrite, machineStore))
                    {
                        try
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                string content = reader.ReadToEnd();
                                Guid guid = Guid.Parse(content);
                                return guid;
                            }
                        }
                        catch (Exception)
                        {
                            _traceManager?.TraceError("Impossible de lire le contenu du fichier MachineIdentifierGuid dans l'isolated storage");
                        }
                    }

                }
            }

            return CreateGuid();
        }

        /// <summary>
        /// Crée le Guid et le stocke dans l'isolated storage.
        /// </summary>
        /// <returns>Le Guid créé</returns>
        private Guid CreateGuid()
        {
            _traceManager?.TraceDebug("MachineMultiFootPrintIdentifierV3.GetIdentifierHash : Création Guid");
            lock (_syncRoot)
            {
                try
                {
                    var machineStore = IsolatedStorageFile.GetMachineStoreForAssembly();
                    if (machineStore == null)
                        throw new IOException("Impossible d'obtenir IsolatedStorageFile.GetMachineStoreForAssembly");

                    using (IsolatedStorageFileStream stream = machineStore.CreateFile(GuidFileName))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            var guid = Guid.NewGuid();
                            writer.Write(guid.ToString(null, System.Globalization.CultureInfo.InvariantCulture));
                            return guid;
                        }
                    }
                }
                catch (Exception)
                {
                    _traceManager?.TraceError("Impossible de lire le contenu du fichier MachineIdentifierGuid dans l'isolated storage");
                    throw;
                }
            }
        }

    }
}
