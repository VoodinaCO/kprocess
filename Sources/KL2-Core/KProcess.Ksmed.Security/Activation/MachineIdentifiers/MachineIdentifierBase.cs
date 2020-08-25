using System;
using System.Security.Cryptography;
using System.Text;

namespace KProcess.Ksmed.Security.Activation.MachineIdentifiers
{
    abstract public class MachineIdentifierBase : IMachineIdentifier
    {
        readonly protected ITraceManager _traceManager;

        private byte[] _IdentifierHash = null;

        virtual public byte[] IdentifierHash
        {
            get
            {
                if (_IdentifierHash == null)
                {
                    _IdentifierHash = GetIdentifierHash();
                }
                return _IdentifierHash;
            }
        }

        public MachineIdentifierBase(ITraceManager traceManager)
        {
            _traceManager = traceManager;
        }

        abstract protected byte[] GetIdentifierHash();

        virtual public bool Match(byte[] hash)
        {
            _traceManager?.TraceDebug("{0}.Match : hash: {1}, IdentifierHash: {2}, hash.Length: {3}, IdentifierHash.Length: {4}",
                this.GetType().Name,
                hash != null ? Convert.ToBase64String(hash) : "null",
                IdentifierHash != null ? Convert.ToBase64String(IdentifierHash) : "null",
                hash != null ? hash.Length : -1,
                IdentifierHash != null ? IdentifierHash.Length : -1
                );

            if (ReferenceEquals(IdentifierHash, hash))
                return true;

            if (IdentifierHash == null || hash == null)
                return false;

            if (IdentifierHash.Length != hash.Length)
                return false;

            for (int n = 0; n < IdentifierHash.Length; n++)
            {
                if (IdentifierHash[n] != hash[n])
                    return false;
            }
            return true;
        }

        protected byte[] ComputeHash(string value)
        {
            MD5 hasher = new MD5CryptoServiceProvider();
            byte[] hash = hasher.ComputeHash(Encoding.ASCII.GetBytes(value));
            return hash;
        }
    }
}
