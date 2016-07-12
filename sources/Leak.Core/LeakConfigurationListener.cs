using Leak.Core.Net;

namespace Leak.Core
{
    public class LeakConfigurationListener
    {
        public LeakConfigurationListener()
        {
        }

        internal bool IsEnabled { get; set; }

        internal PeerNegotiator Negotiator { get; set; }

        internal int Port { get; set; }
    }
}