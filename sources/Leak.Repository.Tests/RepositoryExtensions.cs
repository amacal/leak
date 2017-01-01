using Leak.Common;
using Leak.Events;

namespace Leak.Repository.Tests
{
    public static class RepositoryExtensions
    {
        public static void Write(this RepositoryService service, int piece, byte[] data)
        {
            service.Write(new RepositoryBlockData(new BlockIndex(piece, 0, data.Length), new FixedDataBlock(data)));
        }

        public static void HandleMetadataDiscovered(this RepositoryService service, Metainfo metainfo)
        {
            service.Handle(new MetadataDiscovered
            {
                Hash = service.Hash,
                Metainfo = metainfo
            });
        }
    }
}
