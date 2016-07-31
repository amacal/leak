using Leak.Core.Loop;
using Leak.Core.Network;

namespace Leak.Core.Collector
{
    public static class PeerCollectorMessageExtensions
    {
        public static PeerCollectorMessage ToConnector(this ConnectionLoopMessage message, string type)
        {
            return new PeerCollectorMessage(type, message.Size);
        }

        public static PeerCollectorMessage ToConnector(this NetworkOutgoingMessage message, string type)
        {
            return new PeerCollectorMessage(type, message.Length - 4);
        }
    }
}