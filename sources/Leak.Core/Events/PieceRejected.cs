using Leak.Common;

namespace Leak.Core.Events
{
    public class PieceRejected
    {
        public FileHash Hash;

        public int Piece;
    }
}