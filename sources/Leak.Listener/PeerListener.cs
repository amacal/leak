using System;
using System.Net;
using Leak.Common;
using Leak.Networking;
using Leak.Sockets;

namespace Leak.Listener
{
    public class PeerListener : IDisposable
    {
        private readonly TcpSocket socket;
        private readonly PeerListenerDependencies dependencies;
        private readonly PeerListenerHooks hooks;
        private readonly PeerListenerConfiguration configuration;

        private int assignedPort;

        public PeerListener(PeerListenerDependencies dependencies, PeerListenerHooks hooks, PeerListenerConfiguration configuration)
        {
            this.dependencies = dependencies;
            this.hooks = hooks;
            this.configuration = configuration;

            this.socket = dependencies.Network.New();
        }

        public PeerListenerDependencies Dependencies
        {
            get { return dependencies; }
        }

        public PeerListenerHooks Hooks
        {
            get { return hooks; }
        }

        public PeerListenerConfiguration Configuration
        {
            get { return configuration; }
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

        private void OnAccept(TcpSocketAccept data)
        {
            bool canAcceptMore =
                data.Status != TcpSocketStatus.NotSocket &&
                data.Status != TcpSocketStatus.OperationAborted;

            if (canAcceptMore)
            {
                data.Socket.Accept(OnAccept);
            }

            if (data.Status == TcpSocketStatus.OK)
            {
                IPEndPoint endpoint = data.GetRemote();
                PeerAddress remote = PeerAddress.Parse(endpoint);

                NetworkDirection direction = NetworkDirection.Incoming;
                NetworkConnection connection = dependencies.Network.Create(data.Connection, direction, endpoint);

                hooks.CallConnectionArrived(remote, connection);
            }
            else
            {
                data.Connection?.Dispose();
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}