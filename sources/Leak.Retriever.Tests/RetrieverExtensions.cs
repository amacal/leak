using Leak.Common;
using Leak.Events;

namespace Leak.Retriever.Tests
{
    public static class RetrieverExtensions
    {
        public static void HandleBlockReceived(this RetrieverService service, int index, byte[] data)
        {
            service.Handle(new BlockReceived
            {
                Payload = new FixedDataBlock(data),
                Peer = PeerHash.Random(),
                Hash = service.Hash,
                Piece = index,
                Block = 0,
            });
        }
    }
}
