namespace Leak.Peer.Negotiator
{
    public class HandshakeKeyContainer
    {
        public byte[] Secret { get; set; }

        public HandshakeKey Local { get; set; }

        public HandshakeKey Remote { get; set; }
    }
}