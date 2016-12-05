using Leak.Common;

namespace Leak.Core.Leakage
{
    public class LeakConfiguration
    {
        public LeakConfiguration()
        {
            Peer = PeerHash.Random();
        }

        public int? Port;

        public PeerHash Peer;
    }
}