using Leak.Core.Core;

namespace Leak.Core.Network
{
    public class NetworkPoolSend : LeakTask<NetworkPool>
    {
        private readonly NetworkPoolConnection connection;
        private readonly byte[] data;

        public NetworkPoolSend(NetworkPoolConnection connection, byte[] data)
        {
            this.connection = connection;
            this.data = data;
        }

        public void Execute(NetworkPool context)
        {
            connection.Send(data);
        }
    }
}