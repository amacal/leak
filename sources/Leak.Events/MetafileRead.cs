using Leak.Common;

namespace Leak.Events
{
    public class MetafileRead
    {
        public FileHash Hash;

        public int Piece;

        public int Total;

        public byte[] Payload;
    }
}