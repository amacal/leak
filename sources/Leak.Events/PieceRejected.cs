using Leak.Common;

namespace Leak.Events
{
    public class PieceRejected
    {
        public FileHash Hash;

        public PieceInfo Piece;
    }
}