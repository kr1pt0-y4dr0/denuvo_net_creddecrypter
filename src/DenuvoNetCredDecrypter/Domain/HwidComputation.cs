namespace DenuvoNetCredDecrypter.Domain
{
    /// <summary>
    /// Result of the HWID calculation, exposing the intermediate values for display.
    /// </summary>
    public sealed class HwidComputation
    {
        public HwidComputation(string cpuId, string baseId, string concatenated, string hwid)
        {
            CpuId = cpuId;
            BaseId = baseId;
            Concatenated = concatenated;
            Hwid = hwid;
        }

        public string CpuId { get; }

        public string BaseId { get; }

        public string Concatenated { get; }

        public string Hwid { get; }
    }
}
