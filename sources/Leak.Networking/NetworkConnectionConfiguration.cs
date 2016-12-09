namespace Leak.Networking
{
    public class NetworkConnectionConfiguration
    {
        public NetworkConnectionConfiguration()
        {
            Encryptor = NetworkEncryptor.Nothing;
            Decryptor = NetworkDecryptor.Nothing;
        }

        public NetworkEncryptor Encryptor { get; set; }
        public NetworkDecryptor Decryptor { get; set; }
    }
}