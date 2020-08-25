using Microsoft.Win32;
using System.Linq;

namespace KProcess.Ksmed.Security.Activation.MachineIdentifiers
{
    public static class DockerHelper
    {
        public static bool IsInContainer()
        {
            var baseKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control");
            return baseKey.GetValueNames().Contains("ContainerType");
        }
    }
}
