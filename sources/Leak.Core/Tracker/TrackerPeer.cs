namespace Leak.Core.Tracker
{
    public class TrackerPeer
    {
        private readonly string host;
        private readonly int port;

        public TrackerPeer(string host, int port)
        {
            this.host = host;
            this.port = port;
        }

        public string Host
        {
            get { return host; }
        }

        public int Port
        {
            get { return port; }
        }
    }
}