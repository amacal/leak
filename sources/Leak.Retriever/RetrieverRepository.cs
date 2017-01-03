using Leak.Common;

namespace Leak.Retriever
{
    public interface RetrieverRepository
    {
        void Verify(PieceInfo piece);

        void Write(BlockIndex block, DataBlock data);
    }
}
