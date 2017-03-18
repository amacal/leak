using Leak.Common;

namespace Leak.Client.Swarm
{
    public class SwarmNotification
    {
        public SwarmNotificationType Type { get; set; }

        public PeerHash Peer { get; set; }
        public PeerAddress Remote { get; set; }

        public Bitfield Bitfield { get; set; }
        public PeerState State { get; set; }

        public PieceInfo Piece { get; set; }
        public BlockIndex Block { get; set; }

        public Metainfo Metainfo { get; set; }
        public Size Size { get; set; }
    }
}