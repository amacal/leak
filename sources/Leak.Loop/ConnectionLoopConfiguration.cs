using System.Collections.Generic;

namespace Leak.Peer.Receiver
{
    public class ConnectionLoopConfiguration
    {
        public ConnectionLoopConfiguration()
        {
            Messages = new Dictionary<int, string>
            {
                { 0, "choke" },
                { 1, "unchoke" },
                { 2, "interested" },
                { 4, "have" },
                { 5, "bitfield" },
                { 6, "request" },
                { 7, "piece" },
                { 20, "extended" }
            };
        }

        public Dictionary<int, string> Messages;
    }
}