using Leak.Core.Retriever;

namespace Leak.Core.Client
{
    public class PeerClientPieceVerification
    {
        private readonly ResourcePiece piece;

        public PeerClientPieceVerification(ResourcePiece piece)
        {
            this.piece = piece;
        }

        public int Piece
        {
            get { return piece.Index; }
        }
    }
}