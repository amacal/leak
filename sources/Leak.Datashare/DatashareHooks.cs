using Leak.Events;
using System;

namespace Leak.Datashare
{
    public class DatashareHooks
    {
        public Action<BlockSent> OnBlockSent;
    }
}