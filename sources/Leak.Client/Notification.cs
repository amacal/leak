using Leak.Common;

namespace Leak.Client
{
    public class Notification
    {
        public NotificationType Type { get; set; }
        public string Description { get; set; }

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