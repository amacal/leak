using Leak.Common;
using Leak.Events;
using Leak.Networking.Core;

namespace Leak.Peer.Negotiator
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

        public static void CallHandshakeRejected(this HandshakeNegotiatorHooks hooks, NetworkConnection connection)
        {
            hooks.OnHandshakeRejected?.Invoke(new HandshakeRejected
            {
                Connection = connection
            });
        }

        public static void Start(this HandshakeNegotiator negotiator, NetworkConnection connecton, FileHash hash)
        {
            negotiator.Start(connecton, new HandshakeNegotiatorActiveInstance(PeerHash.Random(), hash, HandshakeOptions.Extended));
        }

        public static void Handle(this HandshakeNegotiator negotiator, NetworkConnection connection, FileHash hash)
        {
            negotiator.Handle(connection, new HandshakeNegotiatorPassiveInstance(PeerHash.Random(), hash, HandshakeOptions.Extended));
        }
    }
}