using System;
using Leak.Common;

namespace Leak.Core.Network
{
    public static class NetworkIncomingMessageExtensions
    {
        public static void BeginOnMessage(this NetworkIncomingMessageHandler handler, NetworkIncomingMessage message)
        {
            Action<NetworkIncomingMessage> onMessage = handler.OnMessage;

            onMessage.BeginInvoke(message, null, null);
        }
    }
}