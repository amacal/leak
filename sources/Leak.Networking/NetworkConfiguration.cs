namespace Leak.Networking
{
    public class NetworkConfiguration
    {
        public NetworkConfiguration()
        {
            Encryptor = NetworkEncryptor.Nothing;
            Decryptor = NetworkDecryptor.Nothing;
        }

        public NetworkEncryptor Encryptor { get; set; }
        public NetworkDecryptor Decryptor { get; set; }
    }
}