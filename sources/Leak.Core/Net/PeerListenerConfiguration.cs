namespace Leak.Core.Net
{
    public class PeerListenerConfiguration
    {
        public int Port { get; set; }

        public PeerNegotiatorHashCollection Hashes { get; set; }

        public PeerNegotiatorCallback Callback { get; set; }

        public PeerNegotiator Negotiator { get; set; }
    }
}