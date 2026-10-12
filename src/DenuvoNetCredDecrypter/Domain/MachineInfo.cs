namespace DenuvoNetCredDecrypter.Domain
{
    /// <summary>
    /// Raw hardware identifiers read from the machine before hashing.
    /// </summary>
    public sealed class MachineInfo
    {
        public string ProcessorId { get; set; } = string.Empty;

        public string ProcessorName { get; set; } = string.Empty;

        public string ProcessorManufacturer { get; set; } = string.Empty;

        public string ProcessorMaxClockSpeed { get; set; } = string.Empty;

        public string BaseBoardManufacturer { get; set; } = string.Empty;

        public string BaseBoardName { get; set; } = string.Empty;

        public string BaseBoardSerialNumber { get; set; } = string.Empty;
    }
}
