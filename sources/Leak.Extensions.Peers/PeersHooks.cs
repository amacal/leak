using System;
using Leak.Glue;

namespace Leak.Extensions.Peers
{
    public class PeersHooks
    {
        public Action<PeersReceived> OnPeersDataReceived;
        public Action<PeersReceived> OnPeersDataSent;
    }
}
