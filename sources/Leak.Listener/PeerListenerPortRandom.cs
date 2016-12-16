using Leak.Sockets;

namespace Leak.Listener
{
    public class PeerListenerPortRandom : PeerListenerPort
    {
        protected override TcpSocketInfo Execute(TcpSocket socket)
        {
            return socket.BindAndInfo();
        }
    }
}