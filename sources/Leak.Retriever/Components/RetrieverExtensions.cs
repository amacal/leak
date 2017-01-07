using Leak.Common;
using Leak.Events;

namespace Leak.Retriever.Components
{
    public static class RetrieverExtensions
    {
        public static void CallBlockHandled(this RetrieverHooks hooks, FileHash hash, PeerHash peer, BlockIndex block)
        {
            hooks.OnBlockHandled?.Invoke(new BlockHandled
            {
                Hash = hash,
                Peer = peer,
                Block = block
            });
        }

        public static void CallBlockRequested(this RetrieverHooks hooks, FileHash hash, PeerHash peer, BlockIndex block)
        {
            hooks.OnBlockRequested?.Invoke(new BlockRequested
            {
                Hash = hash,
                Peer = peer,
                Block = block
            });
        }
    }
}