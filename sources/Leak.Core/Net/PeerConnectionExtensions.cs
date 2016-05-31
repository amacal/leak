using System;

namespace Leak.Core.Net
{
    public static class PeerConnectionExtensions
    {
        public static void Send(this PeerConnection connection, PeerMessageFactory factory)
        {
            connection.Send(factory.GetMessage());
        }

        public static void Receive(this PeerConnection connection, Func<PeerMessage, bool> predicate, Action<PeerMessage> callback)
        {
            Action<PeerMessage> onMessage = null;

            onMessage = message =>
            {
                if (predicate.Invoke(message))
                {
                    callback.Invoke(message);
                }
                else
                {
                    connection.Receive(onMessage);
                }
            };

            connection.ReceiveOrCallback(onMessage);
        }

        public static PeerConnection StartEncryption(this PeerConnection connection, RC4 encryptKey, RC4 decryptKey)
        {
            return new PeerConnection(connection, with =>
            {
                with.Encrypt = encryptKey.Encrypt;
                with.Decrypt = decryptKey.Decrypt;
            });
        }
    }
}