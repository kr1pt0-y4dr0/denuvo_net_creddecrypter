using DenuvoNetCredDecrypter.Domain;

namespace DenuvoNetCredDecrypter.Abstractions
{
    /// <summary>
    /// Persists the retrieved credentials to disk.
    /// </summary>
    public interface ICredentialStore
    {
        string GetPath(string id);

        bool Exists(string id);

        void Save(CredentialRecord record, bool overwrite);
    }
}
