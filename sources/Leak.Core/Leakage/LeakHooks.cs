using Leak.Core.Events;
using System;

namespace Leak.Core.Leakage
{
    public class LeakHooks
    {
        public Action<ListenerStarted> OnListenerStarted;

        public Action<PeerConnected> OnPeerConnected;
    }
}