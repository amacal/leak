using Leak.Core.Retriever;

namespace Leak.Core.Client
{
    public class PeerClientPieceVerification
    {
        private readonly RetrieverPiece piece;

        public PeerClientPieceVerification(RetrieverPiece piece)
        {
            this.piece = piece;
        }

        public int Piece
        {
            get { return piece.Index; }
        }
    }
}