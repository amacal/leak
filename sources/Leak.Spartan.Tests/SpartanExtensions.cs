using Leak.Common;
using Leak.Events;
using Leak.Extensions.Metadata;

namespace Leak.Spartan.Tests
{
    public static class SpartanExtensions
    {
        public static void HandleMetadataMeasured(this SpartanService service, FileHash hash, int size)
        {
            service.Handle(new MetadataMeasured
            {
                Hash = hash,
                Size = size,
                Peer = PeerHash.Random()
            });
        }

        public static void HandleMetadataReceived(this SpartanService service, FileHash hash, int piece, byte[] data)
        {
            service.Handle(new MetadataReceived
            {
                Hash = hash,
                Peer = PeerHash.Random(),
                Piece = piece,
                Data = data
            });
        }

        public static void HandleBlockReceived(this SpartanService service, FileHash hash, int piece, byte[] data)
        {
            service.Handle(new BlockReceived
            {
                Hash = hash,
                Peer = PeerHash.Random(),
                Piece = piece,
                Block = 0,
                Payload = new FixedDataBlock(data)
            });
        }
    }
}
