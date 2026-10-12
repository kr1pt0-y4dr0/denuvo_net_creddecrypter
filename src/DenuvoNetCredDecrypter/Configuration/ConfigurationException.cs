using System;

namespace DenuvoNetCredDecrypter.Configuration
{
    /// <summary>
    /// Raised when the application configuration is missing or invalid.
    /// </summary>
    public sealed class ConfigurationException : Exception
    {
        public ConfigurationException(string message)
            : base(message)
        {
        }
    }
}
