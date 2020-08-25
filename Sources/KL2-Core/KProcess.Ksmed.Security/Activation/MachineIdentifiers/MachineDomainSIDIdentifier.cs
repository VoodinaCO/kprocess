using System;
using System.DirectoryServices.AccountManagement;

namespace KProcess.Ksmed.Security.Activation.MachineIdentifiers
{
    /// <summary>
    /// Fournit le hash du SID de la machine dans son domaine.
    /// Si la machine n'appartient pas à un domaine, le hash est null.
    /// </summary>
    class MachineDomainSIDIdentifier : MachineIdentifierBase, IMachineIdentifier
    {
        public MachineDomainSIDIdentifier(ITraceManager traceManager)
            :base(traceManager)
        {
        }

        protected override byte[] GetIdentifierHash()
        {
            _traceManager?.TraceDebug("MachineDomainSIDIdentifier.GetIdentifierHash");
            try
            {
                System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain();
                _traceManager?.TraceDebug("MachineDomainSIDIdentifier.GetIdentifierHash : Le poste appartient à un domaine");
            }
            catch (Exception e)
            {
                _traceManager?.TraceDebug(e, "MachineDomainSIDIdentifier.GetIdentifierHash : Le poste n'appartient pas à un domaine");
                return null;
            }

            string sid = null;
            try
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
                ComputerPrincipal computer = ComputerPrincipal.FindByIdentity(ctx, IdentityType.Name, Environment.MachineName);
                sid = computer.Sid.Value;

                _traceManager?.TraceDebug("MachineDomainSIDIdentifier.GetIdentifierHash : Le sid de la machine dans le domaine est : {0}", sid ?? String.Empty);
            }
            catch (Exception e)
            {
                _traceManager?.TraceError(e, "MachineDomainSIDIdentifier.GetIdentifierHash : Le sid de la machine dans le domaine est impossible à déterminer");
            }

            if (sid != null)
                return base.ComputeHash(sid);
            else
                return null;
        }

    }
}
