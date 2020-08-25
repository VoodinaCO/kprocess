using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;

namespace KProcess.Ksmed.Security.Activation.MachineIdentifiers
{
    /// <summary>
    /// Fournit un hash regroupant plusieurs numéros de série d'éléments matériels, lorsqu'ils sont accessibles.
    /// </summary>
    class MachineMultiFootPrintIdentifier : MachineIdentifierBase, IMachineIdentifier
    {
        public MachineMultiFootPrintIdentifier(ITraceManager traceManager)
            : base(traceManager)
        {
        }

        protected override byte[] GetIdentifierHash()
        {
            _traceManager?.TraceDebug("MachineMultiFootPrintIdentifier.GetIdentifierHash");

            var hddSerialNumber = GetHDDSerialNumber();
            _traceManager?.TraceDebug("MachineMultiFootPrintIdentifier.GetIdentifierHash : HDDSerialNumber {0}", hddSerialNumber ?? String.Empty);

            var mbSerialNumber = GetMBSerialNumber();
            _traceManager?.TraceDebug("MachineMultiFootPrintIdentifier.GetIdentifierHash : MBSerialNumber {0}", mbSerialNumber ?? String.Empty);

            var cpuID = GetCPUSerialNumber();
            _traceManager?.TraceDebug("MachineMultiFootPrintIdentifier.GetIdentifierHash : CPUSerialNumber {0}", cpuID ?? String.Empty);
            //var macAdresses = GetMacAddresses();

            var concat = string.Concat(
                hddSerialNumber,
                mbSerialNumber,
                cpuID
                //macAdresses
                )
                .Trim();

            _traceManager?.TraceDebug("MachineMultiFootPrintIdentifier.GetIdentifierHash : Concaténé {0}", concat ?? String.Empty);

            if (concat != null)
                return base.ComputeHash(concat);
            else
                return null;

        }

        internal static string GetMacAddresses()
        {
            try
            {
                return
                    string.Concat(
                        ExecuteWMI("SELECT MACAddress FROM Win32_NetworkAdapterConfiguration WHERE MACAddress != null")
                        .Select(o => o["MACAddress"].ToString())
                    );
            }
            catch
            {
            }
            return null;

        }

        internal static string GetCPUSerialNumber()
        {
            try
            {
                var ret = ExecuteWMI("SELECT ProcessorId, UniqueId FROM Win32_Processor").FirstOrDefault();

                var pid = ret["ProcessorId"] != null ? ret["ProcessorId"].ToString() : null;
                var uid = ret["UniqueId"] != null ? ret["UniqueId"].ToString() : null;

                return string.Concat(pid, uid);
            }
            catch
            {
            }
            return null;
        }

        internal static string GetMBSerialNumber()
        {
            try
            {
                return ExecuteWMI("SELECT * FROM Win32_BaseBoard").FirstOrDefault()["SerialNumber"].ToString();
            }
            catch
            {
            }
            return null;
        }

        internal static string GetHDDSerialNumber()
        {
            try
            {
                // Prendre le dossier où est installée l'application
                var di = new DriveInfo(Directory.GetCurrentDirectory());

                ManagementClass disks = new ManagementClass(@"Win32_LogicalDisk");
                foreach (ManagementObject mo in disks.GetInstances())
                {
                    // Si le volume est bien celui en cours
                    if (string.Compare(mo["Name"].ToString().Replace("\\", ""), di.Name.Replace("\\", ""), true) == 0)
                    {
                        foreach (ManagementObject part in mo.GetRelated("Win32_DiskPartition"))
                        {
                            foreach (ManagementObject dd in part.GetRelated("Win32_Diskdrive"))
                            {
                                var deviceId = dd["DeviceID"].ToString();

                                foreach (var pm in ExecuteWMI("SELECT Tag, SerialNumber FROM Win32_PhysicalMedia"))
                                {
                                    if (pm["Tag"].ToString() == deviceId)
                                        return pm["SerialNumber"].ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        internal static IEnumerable<Dictionary<string, object>> ExecuteWMI(string query)
        {
            var searcher = new ManagementObjectSearcher(query);

            foreach (ManagementObject obj in searcher.Get())
                yield return obj.Properties.Cast<PropertyData>().ToDictionary(pd => pd.Name, pd => pd.Value);
        }

    }
}
