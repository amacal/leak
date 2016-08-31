using Leak.Core.Common;

namespace Leak.Core.Telegraph
{
    public class TelegraphConfiguration
    {
        public int Port { get; set; }

        public PeerHash Peer { get; set; }

        public TelegraphCallback Callback { get; set; }
    }
}