namespace Leak.Core.Net
{
    public class PeerClient : PeerChannel
    {
        private readonly PeerConnection connection;
        private readonly PeerCallback callback;

        public PeerClient(PeerConnection connection, PeerCallback callback)
        {
            this.connection = connection;
            this.callback = callback;
        }

        public string Name
        {
            get { return connection.Remote; }
        }

        public void Send(PeerMessageFactory data)
        {
            connection.Send(data.GetMessage());
        }

        public void Start(PeerMessageLoop loop)
        {
            connection.ReceiveOrCallback(loop.Process);
        }
    }
}