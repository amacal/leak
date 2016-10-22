using Leak.Core.Common;

namespace Leak.Core.Client.Events
{
    public class PieceRejectedEvent
    {
        private readonly FileHash hash;
        private readonly PieceInfo piece;

        public PieceRejectedEvent(FileHash hash, PieceInfo piece)
        {
            this.hash = hash;
            this.piece = piece;
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public PieceInfo Piece
        {
            get { return piece; }
        }
    }
}