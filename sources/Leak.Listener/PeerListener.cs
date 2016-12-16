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

                hooks.CallConnectionArrived(remote);
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