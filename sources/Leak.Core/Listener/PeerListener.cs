using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Negotiator;
using Leak.Core.Network;
using Leak.Sockets;
using System.Net;

namespace Leak.Core.Listener
{
    public class PeerListener
    {
        private readonly TcpSocket socket;
        private readonly NetworkPool network;
        private readonly PeerListenerHooks hooks;
        private readonly PeerListenerConfiguration configuration;

        public PeerListener(NetworkPool network, PeerListenerHooks hooks, PeerListenerConfiguration configuration)
        {
            this.network = network;
            this.hooks = hooks;
            this.configuration = configuration;

            socket = network.New();
        }

        public void Start(LeakPipeline pipeline)
        {
            socket.Bind(configuration.Port);
            socket.Listen(8);
            socket.Accept(OnAccept);
            hooks.CallListenerStarted(configuration);
        }

        public void Enable(FileHash hash)
        {
        }

        public void Disable(FileHash hash)
        {
        }

        private void OnAccept(TcpSocketAccept data)
        {
            if (data.Status != TcpSocketStatus.NotSocket)
            {
                data.Socket.Accept(OnAccept);
            }

            if (data.Status == TcpSocketStatus.OK)
            {
                IPEndPoint endpoint = data.GetRemote();
                PeerAddress remote = PeerAddress.Parse(endpoint);

                if (OnConnecting(remote) == false)
                {
                    data.Connection.Dispose();
                    return;
                }

                NetworkConnection connection = network.Create(data.Connection, NetworkDirection.Incoming, endpoint);
                PeerListenerNegotiatorContext context = new PeerListenerNegotiatorContext(configuration, connection);

                Negotiate(context, connection);
            }
            else
            {
                data.Connection.Dispose();
            }
        }

        private bool OnConnecting(PeerAddress remote)
        {
            return true;
        }

        private void Negotiate(PeerListenerNegotiatorContext context, NetworkConnection connection)
        {
            new HandshakeNegotiatorPassive(network, connection, context).Execute();
        }
    }
}