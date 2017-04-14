using Leak.Common;
using Leak.Networking.Core;
using Leak.Peer.Coordinator.Core;

namespace Leak.Peer.Coordinator.Messages
{
    public class PieceIncomingMessage
    {
        private readonly NetworkIncomingMessage inner;

        public PieceIncomingMessage(NetworkIncomingMessage inner)
        {
            this.inner = inner;
        }

        public Piece ToPiece(DataBlockFactory factory)
        {
            DataBlock data = inner.ToBlock(factory, 13, inner.Length - 13);
            int piece = inner[8] + inner[7] * 256 + inner[6] * 256 * 256;
            int offset = inner[12] + inner[11] * 256 + inner[10] * 256 * 256;

            return new Piece(new BlockIndex(piece, offset, data.Length), data);
        }
    }
}