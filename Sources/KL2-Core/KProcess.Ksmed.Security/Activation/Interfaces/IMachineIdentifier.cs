using System;

namespace KProcess.Ksmed.Security.Activation
{
    public interface IMachineIdentifier
    {
        byte[] IdentifierHash { get;}
        bool Match(byte[] identifierHash);
    }
}
