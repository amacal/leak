using System;
using Leak.Events;

namespace Leak.Datashare
{
    public class DatashareHooks
    {
        public Action<BlockSent> OnBlockSent;
    }
}
