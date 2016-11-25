using Leak.Core.Common;

namespace Leak.Core.Events
{
    public class DataRejected
    {
        public FileHash Hash;

        public int Piece;
    }
}