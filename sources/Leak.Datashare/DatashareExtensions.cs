using Leak.Common;
using Leak.Events;

namespace Leak.Data.Share
{
    public static class DatashareExtensions
    {
        public static void CallBlockSent(this DatashareHooks hooks, FileHash hash, PeerHash peer, BlockIndex block)
        {
            hooks.OnBlockSent?.Invoke(new BlockSent
            {
                Hash = hash,
                Peer = peer,
                Block = block
            });
        }
    }
}