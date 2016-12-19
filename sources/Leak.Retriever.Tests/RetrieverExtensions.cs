using Leak.Common;
using Leak.Events;

namespace Leak.Retriever.Tests
{
    public static class RetrieverExtensions
    {
        public static void HandleBlockReceived(this RetrieverService service, FileHash hash, int index, byte[] data)
        {
            service.HandleBlockReceived(new BlockReceived
            {
                Payload = new FixedDataBlock(data),
                Peer = PeerHash.Random(),
                Hash = hash,
                Piece = index,
                Block = 0,
            });
        }
    }
}
