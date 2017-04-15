using Leak.Common;
using Leak.Events;
using Leak.Peer.Coordinator.Events;

namespace Leak.Data.Get
{
    public static class DataGetExtensions
    {
        public static void CallBlockHandled(this DataGetHooks hooks, FileHash hash, PeerHash peer, BlockIndex block)
        {
            hooks.OnBlockHandled?.Invoke(new BlockHandled
            {
                Hash = hash,
                Peer = peer,
                Block = block
            });
        }

        public static void CallBlockRequested(this DataGetHooks hooks, FileHash hash, PeerHash peer, BlockIndex block)
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