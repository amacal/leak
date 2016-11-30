using Leak.Core.Common;

namespace Leak.Core.Events
{
    public class BlockWritten
    {
        public FileHash Hash;

        public int Piece;

        public int Block;

        public int Size;
    }
}