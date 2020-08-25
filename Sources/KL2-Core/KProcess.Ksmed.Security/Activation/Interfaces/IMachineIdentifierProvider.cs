using System;

namespace KProcess.Ksmed.Security.Activation
{
    public interface IMachineIdentifierProvider
    {
        byte[] MachineHash { get; }
        bool Match(byte[] machineHash);
    }
}
