using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Data.Get
{
    public interface DataGetToDataStore
    {
        void Verify(PieceInfo piece);

        void Write(BlockIndex block, DataBlock data);
    }
}