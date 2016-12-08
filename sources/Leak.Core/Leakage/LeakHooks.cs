using System;
using Leak.Events;

namespace Leak.Core.Leakage
{
    public class LeakHooks
    {
        public Action<ListenerStarted> OnListenerStarted;

        public Action<PeerConnected> OnPeerConnected;
    }
}