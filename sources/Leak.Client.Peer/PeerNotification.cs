using Leak.Common;

namespace Leak.Client.Peer
{
    public class PeerNotification
    {
        public PeerNotificationType Type { get; set; }

        public Bitfield Bitfield { get; set; }

        public PeerState State { get; set; }

        public BlockIndex Index { get; set; }

        public Metainfo Metainfo { get; set; }

        public int? Size { get; set; }
    }
}