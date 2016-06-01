namespace Leak.Core.Net
{
    public class PeerClientConfiguration
    {
        public byte[] Hash { get; set; }

        public PeerNegotiatorCallback Callback { get; set; }

        public PeerNegotiator Negotiator { get; set; }

        public PeerHandshakeOptions Options { get; set; }
    }
}