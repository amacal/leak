namespace Leak.Core.Net
{
    public class PeerHandshake
    {
        private readonly PeerConnection connection;
        private readonly PeerHandshakePayload payload;

        public PeerHandshake(PeerConnection connection, PeerHandshakePayload handshake)
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
            PeerListenerChannel channel = new PeerListenerChannel(connection, callback);
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