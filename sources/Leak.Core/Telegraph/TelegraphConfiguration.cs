using Leak.Common;
using Leak.Core.Core;

namespace Leak.Core.Telegraph
{
    public class TelegraphConfiguration
    {
        public int Port { get; set; }

        public PeerHash Peer { get; set; }

        public LeakBus Bus { get; set; }
    }
}