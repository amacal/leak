using System;
using Leak.Events;

namespace Leak.Data.Share
{
    public class DataShareHooks
    {
        public Action<BlockSent> OnBlockSent;
    }
}