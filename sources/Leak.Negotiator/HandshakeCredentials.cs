namespace Leak.Peer.Negotiator
{
    public class HandshakeCredentials
    {
        public byte[] PrivateKey { get; set; }

        public byte[] PublicKey { get; set; }

        public byte[] Padding { get; set; }
    }
}