using Leak.Common;
using Leak.Events;

namespace Leak.Datastore.Tests
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

        public static void HandleMetafileVerified(this RepositoryService service, Metainfo metainfo)
        {
            service.Handle(new MetafileVerified
            {
                Hash = service.Hash,
                Metainfo = metainfo
            });
        }
    }
}