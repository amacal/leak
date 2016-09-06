using Leak.Core.Common;

namespace Leak.Core.Client.Events
{
    public class PieceVerifiedEvent
    {
        private readonly FileHash hash;
        private readonly PieceInfo piece;

        public PieceVerifiedEvent(FileHash hash, PieceInfo piece)
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