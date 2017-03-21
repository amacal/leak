using Leak.Common;

namespace Leak.Data.Get
{
    public interface DataGetToDataStore
    {
        void Verify(PieceInfo piece);

        void Write(BlockIndex block, DataBlock data);
    }
}