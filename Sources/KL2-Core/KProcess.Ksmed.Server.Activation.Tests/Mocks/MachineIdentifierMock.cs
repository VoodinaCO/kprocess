using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Security.Activation;

namespace KProcess.Ksmed.Server.Activation.Tests
{
    class MachineIdentifierProviderMock : IMachineIdentifierProvider
    {
        private bool _matchResult;

        public MachineIdentifierProviderMock() : this(true) { }
        public MachineIdentifierProviderMock(bool matchResult)
        {
            _matchResult = matchResult;
        }

        public byte[] MachineHash
        {
            get { return Encoding.ASCII.GetBytes("0123456789ABCDEF"); }
        }

        public bool Match(byte[] machineHash)
        {
            return _matchResult;
        }
    }
}
