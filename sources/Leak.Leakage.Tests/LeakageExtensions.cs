using Leak.Common;

namespace Leak.Leakage.Tests
{
    public static class LeakageExtensions
    {
        public static LeakRegistrant With(this LeakRegistrant registrant, PeerAddress peer)
        {
            registrant.Peers.Add(peer);
            return registrant;
        }
    }
}
