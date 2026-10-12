using DenuvoNetCredDecrypter.Domain;

namespace DenuvoNetCredDecrypter.Abstractions
{
    /// <summary>
    /// Provides the HWID of the current machine.
    /// </summary>
    public interface IHwidProvider
    {
        HwidComputation GetCurrent();
    }
}
