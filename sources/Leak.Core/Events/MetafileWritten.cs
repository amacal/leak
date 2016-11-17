using Leak.Core.Common;

namespace Leak.Core.Events
{
    public class MetafileWritten
    {
        public FileHash Hash;

        public int Piece;

        public int Size;
    }
}