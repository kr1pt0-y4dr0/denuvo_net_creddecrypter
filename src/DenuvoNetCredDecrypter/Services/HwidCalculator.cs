using System.Security.Cryptography;
using System.Text;
using DenuvoNetCredDecrypter.Domain;

namespace DenuvoNetCredDecrypter.Services
{
    /// <summary>
    /// Reproduces the reference HWID algorithm: CPU + baseboard identifiers,
    /// concatenated and MD5-hashed into an upper-case hex string.
    /// </summary>
    public sealed class HwidCalculator
    {
        public HwidComputation Compute(MachineInfo info)
        {
            var cpuId = info.ProcessorId ?? string.Empty;
            if (string.IsNullOrEmpty(cpuId))
            {
                cpuId = info.ProcessorName ?? string.Empty;
                if (string.IsNullOrEmpty(cpuId))
                {
                    cpuId = info.ProcessorManufacturer ?? string.Empty;
                }

                // The clock speed is only appended on the fallback path.
                cpuId += info.ProcessorMaxClockSpeed ?? string.Empty;
            }

            var baseId = (info.BaseBoardManufacturer ?? string.Empty)
                       + (info.BaseBoardName ?? string.Empty)
                       + (info.BaseBoardSerialNumber ?? string.Empty);

            var concatenated = cpuId + baseId;
            var hwid = ToHexMd5(concatenated);

            return new HwidComputation(cpuId, baseId, concatenated, hwid);
        }

        internal static string ToHexMd5(string value)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(value));
                var builder = new StringBuilder(hash.Length * 2);
                foreach (var b in hash)
                {
                    builder.Append(b.ToString("X2"));
                }

                return builder.ToString();
            }
        }
    }
}
