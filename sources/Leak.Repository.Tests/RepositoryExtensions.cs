using Leak.Common;
using Leak.Events;

namespace Leak.Repository.Tests
{
    public static class RepositoryExtensions
    {
        public static void Read(this RepositoryService service, int piece, int size)
        {
            service.Read(new BlockIndex(piece, 0, size));
        }

        public static void Write(this RepositoryService service, int piece, byte[] data)
        {
            service.Write(new BlockIndex(piece, 0, data.Length), new RepositoryBlock(data));
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
