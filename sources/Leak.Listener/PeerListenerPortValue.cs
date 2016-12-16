using Leak.Sockets;

namespace Leak.Listener
{
    public class PeerListenerPortValue : PeerListenerPort
    {
        private readonly int value;

        public PeerListenerPortValue(int value)
        {
            this.value = value;
        }

        protected override TcpSocketInfo Execute(TcpSocket socket)
        {
            return socket.BindAndInfo(value);
        }
    }
}