using Leak.Common;
using Leak.Extensions.Metadata;

namespace Leak.Metaget.Tests
{
    public static class MetagetExtensions
    {
        public static void HandleMetadataMeasured(this MetagetService service, FileHash hash, int size)
        {
            service.Handle(new MetadataMeasured
            {
                Hash = hash,
                Size = size,
                Peer = PeerHash.Random()
            });
        }

        public static void HandleMetadataReceived(this MetagetService service, FileHash hash, int piece, byte[] data)
        {
            service.Handle(new MetadataReceived
            {
                Hash = hash,
                Piece = piece,
                Data = data,
                Peer = PeerHash.Random()
            });
        }
    }
}
