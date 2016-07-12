using Leak.Core.Net;

namespace Leak.Core
{
    public class LeakConfigurationPeer
    {
        public LeakConfigurationPeer()
        {
        }

        internal string Host { get; set; }

        internal int Port { get; set; }

        internal PeerNegotiator Negotiator { get; set; }
    }
}