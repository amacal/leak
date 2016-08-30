using Leak.Core.Common;
using Leak.Core.Retriever;

namespace Leak.Core.Client.Events
{
    public class PieceVerifiedEvent
    {
        private readonly FileHash hash;
        private readonly RetrieverPiece piece;

        public PieceVerifiedEvent(FileHash hash, RetrieverPiece piece)
        {
            this.hash = hash;
            this.piece = piece;
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public int Piece
        {
            get { return piece.Index; }
        }
    }
}