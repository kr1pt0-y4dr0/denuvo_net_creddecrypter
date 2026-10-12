using System.Management;
using DenuvoNetCredDecrypter.Abstractions;
using DenuvoNetCredDecrypter.Domain;

namespace DenuvoNetCredDecrypter.Services
{
    /// <summary>
    /// Reads hardware identifiers from WMI, mirroring the reference PowerShell
    /// calls to <c>Win32_Processor</c> and <c>Win32_BaseBoard</c>.
    /// </summary>
    public sealed class WmiMachineInfoProvider : IMachineInfoProvider
    {
        public MachineInfo Read()
        {
            var info = new MachineInfo();

            using (var processor = FirstOf("Win32_Processor"))
            {
                if (processor != null)
                {
                    info.ProcessorId = ReadString(processor, "ProcessorId");
                    info.ProcessorName = ReadString(processor, "Name");
                    info.ProcessorManufacturer = ReadString(processor, "Manufacturer");
                    info.ProcessorMaxClockSpeed = ReadString(processor, "MaxClockSpeed");
                }
            }

            using (var board = FirstOf("Win32_BaseBoard"))
            {
                if (board != null)
                {
                    info.BaseBoardManufacturer = ReadString(board, "Manufacturer");
                    info.BaseBoardName = ReadString(board, "Name");
                    info.BaseBoardSerialNumber = ReadString(board, "SerialNumber");
                }
            }

            return info;
        }

        private static ManagementObject? FirstOf(string className)
        {
            try
            {
                var searcher = new ManagementObjectSearcher($"SELECT * FROM {className}");
                try
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        return obj;
                    }
                }
                finally
                {
                    searcher.Dispose();
                }
            }
            catch (ManagementException)
            {
                // The class is unavailable on this machine; fall back to empty values.
            }

            return null;
        }

        private static string ReadString(ManagementBaseObject obj, string property)
        {
            try
            {
                return obj[property]?.ToString() ?? string.Empty;
            }
            catch (ManagementException)
            {
                return string.Empty;
            }
        }
    }
}
