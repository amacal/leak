using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Networking.Tests
{
    public class NullReceiver : NetworkIncomingMessageHandler
    {
        public void OnMessage(NetworkIncomingMessage message)
        {
        }

        public void OnDisconnected()
        {
        }
    }
}