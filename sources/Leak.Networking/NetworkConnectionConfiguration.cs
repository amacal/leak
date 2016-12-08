namespace Leak.Networking
{
    public class NetworkConnectionConfiguration
    {
        public NetworkConnectionEncryptor Encryptor { get; set; }

        public NetworkConnectionDecryptor Decryptor { get; set; }
    }
}