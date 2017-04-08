using Leak.Common;
using Leak.Events;
using Leak.Networking.Core;

namespace Leak.Data.Share
{
    public static class DataShareExtensions
    {
        public static DataBlock Shared(this DataBlock block, int count)
        {
            return new DataShareBlock(block, count);
        }

        public static void CallBlockSent(this DataShareHooks hooks, FileHash hash, PeerHash peer, BlockIndex block)
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