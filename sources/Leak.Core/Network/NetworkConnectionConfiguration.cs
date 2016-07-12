namespace Leak.Core.Network
{
    public class NetworkConnectionConfiguration
    {
        public NetworkConnectionEncryptor Encryptor { get; set; }

        public NetworkConnectionDecryptor Decryptor { get; set; }
    }
}