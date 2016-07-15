using FluentAssertions;
using Leak.Core.Net;
using Leak.Core.Network;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Leak.Core.Tests.Net
{
    [TestFixture]
    public class PeerNegotiatorEncryptedTests
    {
        private Socket iSocket;
        private Socket lSocket;

        [SetUp]
        public void OnSetUp()
        {
            iSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            lSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        [TearDown]
        public void OnTearDown()
        {
            iSocket.Close();
            lSocket.Close();
        }

        [Test, Timeout(1000)]
        public void CanTransferSomeDataAfterNegotiation()
        {
            byte[] response = null;
            byte[] request = new byte[] { 1, 2, 3, 4, 5, 6, 7 };

            byte[] hash = Bytes.Random(20);
            TimeSpan timeout = TimeSpan.FromSeconds(1);

            IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, 12345);
            SemaphoreSlim semaphore = new SemaphoreSlim(0);

            PeerNegotiator negotiator = new PeerNegotiatorEncrypted(with =>
            {
            });

            lSocket.Bind(endpoint);
            lSocket.Listen(1);
            lSocket.BeginAccept(x =>
            {
                Socket rSocket = lSocket.EndAccept(x);
                NetworkConnection rConnection = new NetworkConnection(rSocket, NetworkConnectionDirection.Incoming);
                Context rContext = new Context(hash, rConnection, connection =>
                {
                    connection.Receive(message =>
                    {
                        response = message.ToBytes();
                        semaphore.Release();
                    });
                });

                negotiator.Passive(rContext);
            }, null);

            iSocket.Connect(endpoint);
            NetworkConnection iConnection = new NetworkConnection(iSocket, NetworkConnectionDirection.Outgoing);
            Context iContext = new Context(hash, iConnection, connection =>
            {
                connection.Send(new NetworkOutgoingMessageBytes(request));
            });

            negotiator.Active(iContext);
            semaphore.Wait(timeout);

            response.Should().Equal(request);
        }

        private class Context : PeerNegotiatorActiveContext, PeerNegotiatorPassiveContext
        {
            private readonly byte[] hash;
            private readonly NetworkConnection connection;
            private readonly Action<NetworkConnection> callback;

            public Context(byte[] hash, NetworkConnection connection, Action<NetworkConnection> callback)
            {
                this.connection = connection;
                this.callback = callback;
                this.hash = hash;
            }

            public byte[] Hash
            {
                get { return hash; }
            }

            public NetworkConnection Connection
            {
                get { return connection; }
            }

            public PeerHandshakeOptions Options
            {
                get { return PeerHandshakeOptions.None; }
            }

            public PeerNegotiatorHashCollection Hashes
            {
                get { return new PeerNegotiatorHashCollection(hash); }
            }

            public void Continue(PeerHandshakePayload handshake, NetworkConnection connection)
            {
                callback.Invoke(connection);
            }

            public void Terminate()
            {
            }
        }
    }
}