using Leak.Core.Common;

namespace Leak.Core.Events
{
    public class DataWritten
    {
        public FileHash Hash;

        public int Piece;

        public int Block;

        public int Size;
    }
}