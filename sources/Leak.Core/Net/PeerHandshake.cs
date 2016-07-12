using Leak.Core.Network;

namespace Leak.Core.Net
{
    public class PeerHandshake
    {
        private readonly NetworkConnection connection;
        private readonly PeerHandshakePayload payload;

        public PeerHandshake(NetworkConnection connection, PeerHandshakePayload handshake)
        {
            this.connection = connection;
            this.payload = handshake;
        }

        public byte[] Hash
        {
            get { return payload.Hash; }
        }

        public byte[] Peer
        {
            get { return payload.Peer; }
        }

        public PeerHandshakeOptions Options
        {
            get { return payload.Options; }
        }

        public void Accept(PeerCallback callback)
        {
            PeerChannelImpl channel = new PeerChannelImpl(connection, callback, payload.Hash);
            PeerMessageLoop loop = new PeerMessageLoop(channel, connection, with =>
            {
                with.Callback = callback;
            });

            callback.OnAttached(channel);
            channel.Start(loop);
        }

        public void Reject()
        {
        }
    }
}