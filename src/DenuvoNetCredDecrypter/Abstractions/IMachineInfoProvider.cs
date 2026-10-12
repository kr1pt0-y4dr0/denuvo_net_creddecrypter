namespace DenuvoNetCredDecrypter.Abstractions
{
    /// <summary>
    /// Reads the raw hardware identifiers used to build the HWID.
    /// </summary>
    public interface IMachineInfoProvider
    {
        Domain.MachineInfo Read();
    }
}
