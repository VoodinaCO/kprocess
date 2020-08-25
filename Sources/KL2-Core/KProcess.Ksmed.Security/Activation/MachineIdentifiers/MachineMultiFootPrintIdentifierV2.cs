using System;

namespace KProcess.Ksmed.Security.Activation.MachineIdentifiers
{
    /// <summary>
    /// Fournit un hash regroupant plusieurs numéros de série d'éléments matériels, lorsqu'ils sont accessibles.
    /// </summary>
    class MachineMultiFootPrintIdentifierV2 : MachineIdentifierBase, IMachineIdentifier
    {
        public MachineMultiFootPrintIdentifierV2(ITraceManager traceManager)
            : base(traceManager)
        {
        }

        protected override byte[] GetIdentifierHash()
        {
            _traceManager?.TraceDebug("MachineMultiFootPrintIdentifierV2.GetIdentifierHash");

            var mbSerialNumber = MachineMultiFootPrintIdentifier.GetMBSerialNumber();
            _traceManager?.TraceDebug("MachineMultiFootPrintIdentifierV2.GetIdentifierHash : MBSerialNumber {0}", mbSerialNumber ?? String.Empty);

            var cpuID = MachineMultiFootPrintIdentifier.GetCPUSerialNumber();
            _traceManager?.TraceDebug("MachineMultiFootPrintIdentifierV2.GetIdentifierHash : CPUSerialNumber {0}", cpuID ?? String.Empty);

            var concat = string.Concat(
                mbSerialNumber,
                cpuID
                )
                .Trim();

            _traceManager?.TraceDebug("MachineMultiFootPrintIdentifierV2.GetIdentifierHash : Concaténé {0}", concat ?? String.Empty);

            if (concat != null)
                return base.ComputeHash(concat);
            else
                return null;
        }

    }
}
