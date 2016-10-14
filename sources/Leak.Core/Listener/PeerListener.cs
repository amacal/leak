using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Negotiator;
using Leak.Core.Network;
using Leak.Suckets;
using System;
using System.Net;

namespace Leak.Core.Listener
{
    public class PeerListener
    {
        private readonly TcpSocket socket;
        private readonly PeerListenerConfiguration configuration;

        public PeerListener(Action<PeerListenerConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Port = 8080;
                with.Callback = new PeerListenerCallbackNothing();
                with.Peer = new PeerHash(Bytes.Random(20));
                with.Pool = new NetworkPool();
            });

            socket = configuration.Pool.New();
        }

        public void Start(LeakPipeline pipeline)
        {
            int port = configuration.Port;
            PeerHash peer = configuration.Peer;

            socket.Bind(port);
            socket.Listen(8);

            socket.Accept(OnAccept);
            configuration.Callback.OnStarted(new PeerListenerStarted(peer, port));
        }

        private void OnAccept(TcpSocketAccept data)
        {
            if (data.Status == TcpSocketStatus.OK)
            {
                data.Socket.Accept(OnAccept);

                if (OnConnecting(data.GetRemote()) == false)
                {
                    data.Connection.Dispose();
                    return;
                }

                TcpSocket accepted = data.Connection;
                IPEndPoint remote = data.GetRemote();

                NetworkConnection connection = configuration.Pool.Create(accepted, NetworkDirection.Incoming, remote);
                PeerListenerNegotiatorContext context = new PeerListenerNegotiatorContext(configuration, connection);

                OnConnected(connection);
                Negotiate(context, connection);
            }
            else
            {
                data.Connection.Dispose();
            }
        }

        private bool OnConnecting(IPEndPoint endpoint)
        {
            bool accepted = true;

            NetworkConnectionInfo connection = new NetworkConnectionInfo(endpoint.ToString(), NetworkDirection.Incoming);
            PeerListenerConnecting connecting = new PeerListenerConnecting(connection, () => accepted = false);

            configuration.Callback.OnConnecting(connecting);
            return accepted;
        }

        private void OnConnected(NetworkConnection connection)
        {
            configuration.Callback.OnConnected(connection);
        }

        private void Negotiate(PeerListenerNegotiatorContext context, NetworkConnection connection)
        {
            new HandshakeNegotiatorPassive(configuration.Pool, connection, context).Execute();
        }
    }
}