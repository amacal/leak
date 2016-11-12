using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Listener.Events;
using Leak.Core.Negotiator;
using Leak.Core.Network;
using Leak.Sockets;
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
            });

            socket = configuration.Pool.New();
        }

        public void Start(LeakPipeline pipeline)
        {
            socket.Bind(configuration.Port);
            socket.Listen(8);
            socket.Accept(OnAccept);

            configuration.Bus.Publish("listener-started", new ListenerStarted
            {
                Local = configuration.Peer,
                Port = configuration.Port
            });
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

                configuration.Bus.Publish("listener-accepting", new ListenerAccepted
                {
                    Local = configuration.Peer,
                    Remote = remote
                });

                if (OnConnecting(remote) == false)
                {
                    data.Connection.Dispose();
                    return;
                }

                NetworkConnection connection = configuration.Pool.Create(data.Connection, NetworkDirection.Incoming, endpoint);
                PeerListenerNegotiatorContext context = new PeerListenerNegotiatorContext(configuration, connection);

                configuration.Bus.Publish("listener-accepted", new ListenerAccepted
                {
                    Local = configuration.Peer,
                    Remote = remote,
                    Connection = connection
                });

                Negotiate(context, connection);
            }
            else
            {
                data.Connection.Dispose();
            }
        }

        private bool OnConnecting(PeerAddress remote)
        {
            bool accepted = true;

            NetworkConnectionInfo connection = new NetworkConnectionInfo(remote, NetworkDirection.Incoming);
            PeerListenerConnecting connecting = new PeerListenerConnecting(connection, () => accepted = false);

            configuration.Callback.OnConnecting(connecting);
            return accepted;
        }

        private void Negotiate(PeerListenerNegotiatorContext context, NetworkConnection connection)
        {
            new HandshakeNegotiatorPassive(configuration.Pool, connection, context).Execute();
        }
    }
}