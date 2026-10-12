namespace DenuvoNetCredDecrypter.Domain
{
    /// <summary>
    /// What ends up being persisted to <c>&lt;id&gt;.txt</c>.
    /// </summary>
    public sealed class CredentialRecord
    {
        public CredentialRecord(string id, string login, string password, string hwid, string hardwareInfo)
        {
            Id = id;
            Login = login;
            Password = password;
            Hwid = hwid;
            HardwareInfo = hardwareInfo;
        }

        public string Id { get; }

        public string Login { get; }

        public string Password { get; }

        public string Hwid { get; }

        public string HardwareInfo { get; }
    }
}
