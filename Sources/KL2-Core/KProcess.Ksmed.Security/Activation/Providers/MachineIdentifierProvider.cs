using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Security.Activation.MachineIdentifiers;

namespace KProcess.Ksmed.Security.Activation.Providers
{
    public class MachineIdentifierProvider : IMachineIdentifierProvider
    {
        readonly ITraceManager _traceManager;

        public IMachineIdentifier NewMachineIdentifier { get; private set; }
        public List<IMachineIdentifier> OldMachineIdentifiersCheck { get; private set; }

        public MachineIdentifierProvider(ITraceManager traceManager, IMachineIdentifier newMachineId, params IMachineIdentifier[] oldMachineIdCheck)
        {
            _traceManager = traceManager;
            NewMachineIdentifier = newMachineId;
            OldMachineIdentifiersCheck = new List<IMachineIdentifier>(oldMachineIdCheck);
        }

        public bool Match(byte[] machineHash)
        {
            _traceManager?.TraceDebug("MachineIdentifierProvider.Match(stored hash: {0})",
                machineHash != null ? Convert.ToBase64String(machineHash) : "null");

            var identifiers = EnumerableExt.Concat(NewMachineIdentifier).Concat(OldMachineIdentifiersCheck);

            using (MemoryStream stream = new MemoryStream(machineHash))
            {
                byte[] hash = new byte[16];
                if (stream.Read(hash, 0, 16) != 16)
                    return false;

                foreach (var identifier in identifiers)
                {
                    if (identifier.Match(hash))
                        return true;
                }
            }

            return false;
        }

        public byte[] MachineHash
        {
            get
            {
                _traceManager?.TraceDebug("MachineIdentifierProvider.MachineHash");
                if (NewMachineIdentifier.IdentifierHash != null)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        _traceManager?.TraceDebug("MachineIdentifierProvider.MachineHash : {0}",
                           Convert.ToBase64String(NewMachineIdentifier.IdentifierHash));
                        stream.Write(NewMachineIdentifier.IdentifierHash, 0, 16);
                        return stream.ToArray();
                    }
                }
                else
                    return null;
            }
        }
    }
}
