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
                Payload = new RetrieverBlock(data),
                Peer = PeerHash.Random(),
                Hash = service.Hash,
                Block = new BlockIndex(index, 0, data.Length)
            });
        }
    }
}
