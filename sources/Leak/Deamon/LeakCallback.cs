using Leak.Core.Client;

namespace Leak.Deamon
{
    public class LeakCallback : PeerClientCallbackBase
    {
        public readonly LeakMonitor monitor;

        public LeakCallback(LeakMonitor monitor)
        {
            this.monitor = monitor;
        }
    }
}