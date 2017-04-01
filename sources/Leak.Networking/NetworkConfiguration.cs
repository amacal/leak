namespace Leak.Networking
{
    public class NetworkConfiguration
    {
        public NetworkOutgoingEncryptor Encryptor { get; set; }
        public NetworkIncomingDecryptor Decryptor { get; set; }
    }
}