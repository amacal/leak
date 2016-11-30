using Leak.Core.Common;

namespace Leak.Core.Events
{
    public class PieceAccepted
    {
        public FileHash Hash;

        public int Piece;
    }
}