using Leak.Common;
using Leak.Events;

namespace Leak.Negotiator
{
    public static class HandshakeNegotiatorExtensions
    {
        public static void CallHandshakeCompleted(this HandshakeNegotiatorHooks hooks, NetworkConnection connection, Handshake handshake)
        {
            hooks.OnHandshakeCompleted?.Invoke(new HandshakeCompleted
            {
                Hash = handshake.Hash,
                Handshake = handshake,
                Connection = connection
            });
        }

        public static void Start(this HandshakeNegotiator negotiator, NetworkConnection connecton, FileHash hash)
        {
            negotiator.Start(connecton, new HandshakeNegotiatorActiveInstance(hash, PeerHash.Random(), HandshakeOptions.Extended));
        }

        public static void Handle(this HandshakeNegotiator negotiator, NetworkConnection connection, FileHash hash)
        {
            negotiator.Handle(connection, new HandshakeNegotiatorPassiveInstance(new FileHashCollection(hash), PeerHash.Random(), HandshakeOptions.Extended));
        }
    }
}
