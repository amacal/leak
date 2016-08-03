using System;

namespace Leak.Core.Common
{
    public class PeerAddress
    {
        private readonly string host;
        private readonly int port;

        public PeerAddress(string host, int port)
        {
            this.host = host;
            this.port = port;
        }

        public static PeerAddress Parse(string remote)
        {
            string[] parts = remote.Split(':');
            int port = Int32.Parse(parts[1]);

            return new PeerAddress(parts[0], port);
        }

        public string Host
        {
            get { return host; }
        }

        public int Port
        {
            get { return port; }
        }

        public override string ToString()
        {
            return $"{host}:{port}";
        }

        public override int GetHashCode()
        {
            return host.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            PeerAddress other = obj as PeerAddress;

            return other != null && other.host == host && other.port == port;
        }
    }
}