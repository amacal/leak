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
            configuration = configurer.Configure(with =>
            {
                with.Port = 8080;
                with.Callback = new PeerListenerCallbackNothing();
                with.Peer = new PeerHash(Bytes.Random(20));
                with.Pool = new NetworkPool();
            });

            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            int port = configuration.Port;
            PeerHash peer = configuration.Peer;

            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Listen(8);

            socket.BeginAccept(OnAccept, this);
            configuration.Callback.OnStarted(new PeerListenerStarted(peer, port));
        }

        private void OnAccept(IAsyncResult result)
        {
            try
            {
                Socket accepted = socket.EndAccept(result);

                if (OnConnecting(accepted) == false)
                {
                    accepted.Close();
                    accepted.Dispose();

                    return;
                }

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

        private bool OnConnecting(Socket incoming)
        {
            bool accepted = true;

            NetworkConnectionInfo connection = configuration.Pool.Info(incoming, NetworkDirection.Incoming);
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