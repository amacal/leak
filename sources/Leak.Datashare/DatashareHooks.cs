using System;
using Leak.Events;

namespace Leak.Data.Share
{
    public class DatashareHooks
    {
        public Action<BlockSent> OnBlockSent;
    }
}