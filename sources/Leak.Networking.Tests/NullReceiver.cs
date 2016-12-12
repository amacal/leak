using Leak.Common;

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