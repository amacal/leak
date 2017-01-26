using Leak.Common;

namespace Leak.Dataget
{
    public interface RetrieverRepository
    {
        void Verify(PieceInfo piece);

        void Write(BlockIndex block, DataBlock data);
    }
}