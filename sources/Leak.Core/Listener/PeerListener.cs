using Leak.Common;
using Leak.Core.Negotiator;
using Leak.Core.Network;
using Leak.Sockets;
using System;
using System.Net;

namespace Leak.Core.Listener
{
    public class PeerListener : IDisposable
    {
        private readonly TcpSocket socket;
        private readonly NetworkPool network;
        private readonly PeerListenerHooks hooks;
        private readonly PeerListenerConfiguration configuration;
        private readonly FileHashCollection hashes;

        private int assignedPort;

        public PeerListener(NetworkPool network, PeerListenerHooks hooks, PeerListenerConfiguration configuration)
        {
            this.network = network;
            this.hooks = hooks;
            this.configuration = configuration;

            this.socket = network.New();
            hashes = new FileHashCollection();
        }

        public void Start()
        {
            assignedPort = configuration.Port.Bind(socket);

            socket.Listen(8);
            socket.Accept(OnAccept);

            hooks.CallListenerStarted(configuration, assignedPort);
        }

        public void Stop()
        {
            socket.Dispose();
        }

        public void Enable(FileHash hash)
        {
            hashes.Add(hash);
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

                hooks.CallConnectionArrived(remote);

                if (OnConnecting(remote) == false)
                {
                    data.Connection.Dispose();
                    return;
                }

                NetworkConnection connection = network.Create(data.Connection, NetworkDirection.Incoming, endpoint);
                PeerListenerNegotiatorContext context = new PeerListenerNegotiatorContext(connection, hashes, hooks, configuration);

                Negotiate(context, connection);
            }
            else
            {
                data.Connection?.Dispose();
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

        public void Dispose()
        {
            Stop();
        }
    }
}