using Leak.Common;

namespace Leak.Data.Get
{
    public interface RetrieverRepository
    {
        void Verify(PieceInfo piece);

        void Write(BlockIndex block, DataBlock data);
    }
}