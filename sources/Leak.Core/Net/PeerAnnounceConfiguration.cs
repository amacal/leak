namespace Leak.Core.Net
{
    public class PeerAnnounceConfiguration
    {
        public byte[] Hash { get; set; }

        public byte[] Peer { get; set; }

        public byte[] Address { get; set; }

        public int Port { get; set; }
    }
}