using Leak.Common;

namespace Leak.Events
{
    public class PieceAccepted
    {
        public FileHash Hash;

        public PieceInfo Piece;
    }
}