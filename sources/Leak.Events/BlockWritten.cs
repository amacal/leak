using Leak.Common;

namespace Leak.Events
{
    public class BlockWritten
    {
        public FileHash Hash;

        public int Piece;

        public int Block;

        public int Size;
    }
}