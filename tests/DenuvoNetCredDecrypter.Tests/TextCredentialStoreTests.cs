using System;
using System.IO;
using DenuvoNetCredDecrypter.Domain;
using DenuvoNetCredDecrypter.Services;
using Xunit;

namespace DenuvoNetCredDecrypter.Tests
{
    public sealed class TextCredentialStoreTests : IDisposable
    {
        private readonly string _directory;
        private readonly TextCredentialStore _store;

        public TextCredentialStoreTests()
        {
            _directory = Path.Combine(Path.GetTempPath(), "dncd-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_directory);
            _store = new TextCredentialStore(_directory);
        }

        [Fact]
        public void Writes_labelled_lines_to_the_id_named_file()
        {
            var record = new CredentialRecord("my-id", "alice", "pw", "HWID", "some hardware");

            _store.Save(record, overwrite: false);

            var path = Path.Combine(_directory, "my-id.txt");
            Assert.True(File.Exists(path));
            var lines = File.ReadAllLines(path);
            Assert.Equal("login: alice", lines[0]);
            Assert.Equal("password: pw", lines[1]);
            Assert.Equal("hwid: HWID", lines[2]);
            Assert.Equal("hardware info: some hardware", lines[3]);
        }

        [Fact]
        public void Exists_reflects_the_file_system()
        {
            Assert.False(_store.Exists("absent"));
            _store.Save(new CredentialRecord("present", "l", "p", "h", "i"), overwrite: false);
            Assert.True(_store.Exists("present"));
        }

        [Fact]
        public void Refuses_to_overwrite_when_not_allowed()
        {
            var record = new CredentialRecord("dup", "l", "p", "h", "i");
            _store.Save(record, overwrite: false);

            Assert.Throws<IOException>(() => _store.Save(record, overwrite: false));
        }

        [Fact]
        public void Sanitizes_invalid_file_name_characters()
        {
            Assert.Equal("a_b_c", TextCredentialStore.Sanitize("a:b?c"));
            Assert.Equal("id", TextCredentialStore.Sanitize("  id  "));
            Assert.Throws<ArgumentException>(() => TextCredentialStore.Sanitize("   "));
        }

        public void Dispose()
        {
            if (Directory.Exists(_directory))
            {
                Directory.Delete(_directory, recursive: true);
            }
        }
    }
}
