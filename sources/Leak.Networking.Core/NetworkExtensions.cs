using System;

namespace Leak.Networking.Core
{
    public static class NetworkExtensions
    {
        public static void ReceiveIf(this NetworkConnection connection, Action<NetworkIncomingMessage> callback, int bytes)
        {
            connection.Receive(new NetworkBytesHandler(callback, bytes));
        }

        public static void ReceiveIf(this NetworkConnection connection, Action<NetworkIncomingMessage> callback, Func<NetworkIncomingMessage, bool> predicate)
        {
            connection.Receive(new NetworkPredicateHandler(callback, predicate, null));
        }

        public static void ReceiveIf(this NetworkConnection connection, Action<NetworkIncomingMessage> callback, Func<NetworkIncomingMessage, bool> predicate, Action onDisconnected)
        {
            connection.Receive(new NetworkPredicateHandler(callback, predicate, onDisconnected));
        }

        public static NetworkIncomingMessage Restricted(this NetworkIncomingMessage message, int size)
        {
            return new NetworkRestrictedMessage(message, size);
        }
    }
}