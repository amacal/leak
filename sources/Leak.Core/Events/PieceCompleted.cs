using Leak.Common;

namespace Leak.Core.Events
{
    public class PieceCompleted
    {
        public FileHash Hash;

        public int Piece;
    }
}