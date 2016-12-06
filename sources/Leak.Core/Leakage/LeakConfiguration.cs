using Leak.Common;

namespace Leak.Core.Leakage
{
    public class LeakConfiguration
    {
        public LeakConfiguration()
        {
            Peer = PeerHash.Random();
            Port = LeakPort.Nothing;
        }

        public LeakPort Port;

        public PeerHash Peer;
    }
}