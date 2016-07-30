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
            this.socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            this.configuration = new PeerListenerConfiguration
            {
                Port = 8080,
                Callback = new PeerListenerCallbackToNothing(),
                Peer = new PeerHash(Bytes.Random(20))
            };

            configurer.Invoke(configuration);
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
                NetworkConnection connection = new NetworkConnection(accepted, NetworkConnectionDirection.Incoming);
                PeerListenerNegotiatorContext context = new PeerListenerNegotiatorContext(configuration, connection);

                OnConnected(connection);
                Negotiate(context, connection);
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

        private static void Negotiate(PeerListenerNegotiatorContext context, NetworkConnection connection)
        {
            new HandshakeNegotiatorPassive(connection, context).Execute();
        }
    }
}