using Leak.Core.Common;
using Leak.Core.Negotiator;
using Leak.Core.Network;
using System;
using System.Net;
using System.Net.Sockets;

namespace Leak.Core.Listener
{
    public class PeerListener
    {
        private readonly Socket socket;
        private readonly PeerListenerConfiguration configuration;

        public PeerListener(Action<PeerListenerConfiguration> configurer)
        {
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            configuration = configurer.Configure(with =>
            {
                with.Port = 8080;
                with.Callback = new PeerListenerCallbackNothing();
                with.Peer = new PeerHash(Bytes.Random(20));
                with.Pool = new NetworkPool();
            });
        }

        public void Start()
        {
            socket.Bind(new IPEndPoint(IPAddress.Any, configuration.Port));
            socket.Listen(8);

            socket.BeginAccept(OnAccept, this);
            configuration.Callback.OnStarted();
        }

        public void Stop()
        {
        }

        private void OnAccept(IAsyncResult result)
        {
            try
            {
                Socket accepted = socket.EndAccept(result);
                NetworkConnection connection = configuration.Pool.Create(accepted, NetworkDirection.Incoming);
                PeerListenerNegotiatorContext context = new PeerListenerNegotiatorContext(configuration, connection);

                OnConnected(connection);
                Negotiate(context, connection);
            }
            catch (SocketException)
            {
            }
            finally
            {
                socket.BeginAccept(OnAccept, this);
            }
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