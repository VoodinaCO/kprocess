using System;

namespace KProcess.Ksmed.Security.Activation.MachineIdentifiers
{
    public class MachineNameIdentifier : MachineIdentifierBase, IMachineIdentifier
    {
        public MachineNameIdentifier(ITraceManager traceManager)
            : base(traceManager)
        {
        }

        protected override byte[] GetIdentifierHash()
        {
            return base.ComputeHash(Environment.MachineName);
        }
    }
}
