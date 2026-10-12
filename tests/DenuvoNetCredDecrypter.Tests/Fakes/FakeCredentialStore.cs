using DenuvoNetCredDecrypter.Abstractions;
using DenuvoNetCredDecrypter.Domain;

namespace DenuvoNetCredDecrypter.Tests.Fakes
{
    internal sealed class FakeCredentialStore : ICredentialStore
    {
        public bool ExistsResult { get; set; }

        public string PathResult { get; set; } = "fake-path.txt";

        public CredentialRecord? SavedRecord { get; private set; }

        public bool SavedOverwrite { get; private set; }

        public string GetPath(string id) => PathResult;

        public bool Exists(string id) => ExistsResult;

        public void Save(CredentialRecord record, bool overwrite)
        {
            SavedRecord = record;
            SavedOverwrite = overwrite;
        }
    }
}
