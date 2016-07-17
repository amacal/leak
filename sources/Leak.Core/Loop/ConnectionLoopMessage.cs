using Leak.Core.Network;

namespace Leak.Core.Loop
{
    public class ConnectionLoopMessage
    {
        private readonly NetworkIncomingMessage incoming;

        public ConnectionLoopMessage(NetworkIncomingMessage incoming)
        {
            this.incoming = incoming;
        }
    }
}