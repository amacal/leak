namespace Leak.Core.Net
{
    public class PeerCredentials
    {
        public byte[] PrivateKey { get; set; }

        public byte[] PublicKey { get; set; }

        public byte[] Padding { get; set; }
    }
}