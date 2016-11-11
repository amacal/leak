using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Network;

namespace Leak.Core.Listener
{
    public class PeerListenerConfiguration
    {
        public int Port { get; set; }

        public PeerHash Peer { get; set; }

        public FileHashCollection Hashes { get; set; }

        public PeerListenerCallback Callback { get; set; }

        public LeakBus Bus { get; set; }

        public NetworkPool Pool { get; set; }

        public bool Extensions { get; set; }
    }
}