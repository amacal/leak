using Leak.Core.Messages;
using Leak.Core.Metadata;

namespace Leak.Core.Repository
{
    public interface ResourceRepository
    {
        MetainfoProperties Properties { get; }

        Bitfield Initialize();

        ResourceRepository WithMetainfo(out Metainfo metainfo);

        void SetPiece(int piece, int block, byte[] data);

        bool SetMetadata(int piece, byte[] data);

        bool Verify(int piece);
    }
}