using System;
using Leak.Events;
using Leak.Extensions.Peers;

namespace Leak.Leakage
{
    public class LeakHooks
    {
        public Action<ListenerStarted> OnListenerStarted;

        public Action<PeerConnected> OnPeerConnected;

        public Action<PeersReceived> OnPeerListReceived;
    }
}