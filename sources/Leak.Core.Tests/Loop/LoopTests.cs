using FluentAssertions;
using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Listener;
using Leak.Core.Loop;
using Leak.Core.Network;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Leak.Core.Tests.Loop
{
    [TestFixture]
    public class LoopTests
    {
        [Test]
        public void CanExchangeKeepAliveMessages()
        {
            FileHash hash = new FileHash(Bytes.Random(20));
            LoopCallback callback = new LoopCallback();

            ConnectionLoop loop = new ConnectionLoop(with =>
            {
                with.Callback = callback;
            });

            PeerListener listener = new PeerListener(with =>
            {
                with.Port = 8080;
                with.Callback = new ListenerCallback(loop);
                with.Hashes = new FileHashCollection(hash);
            });

            PeerConnector connector = new PeerConnector(with =>
            {
                with.Hash = hash;
                with.Callback = new ConnectorCallback(loop);
            });

            listener.Start();
            connector.ConnectTo("localhost", 8080);

            Thread.Sleep(TimeSpan.FromSeconds(2));
            callback.KeepAlive.Should().HaveCount(2);
        }

        public class LoopCallback : ConnectionLoopCallback
        {
            private readonly List<ConnectionLoopChannel> keepAlive;

            public LoopCallback()
            {
                this.keepAlive = new List<ConnectionLoopChannel>();
            }

            public IEnumerable<ConnectionLoopChannel> KeepAlive
            {
                get { return keepAlive; }
            }

            public void OnConnected(ConnectionLoopChannel channel)
            {
            }

            public void OnKeepAlive(ConnectionLoopChannel channel)
            {
                keepAlive.Add(channel);
            }

            public void OnUnchoke(ConnectionLoopChannel channel, ConnectionLoopMessage message)
            {
            }

            public void OnInterested(ConnectionLoopChannel channel, ConnectionLoopMessage message)
            {
            }

            public void OnBitfield(ConnectionLoopChannel channel, ConnectionLoopMessage message)
            {
            }

            public void OnPiece(ConnectionLoopChannel channel, ConnectionLoopMessage message)
            {
            }

            public void OnException(ConnectionLoopChannel channel, Exception ex)
            {
            }

            public void OnDisconnected(ConnectionLoopChannel channel)
            {
            }
        }

        public class ListenerCallback : PeerListenerCallback
        {
            private readonly ConnectionLoop loop;

            public ListenerCallback(ConnectionLoop loop)
            {
                this.loop = loop;
            }

            public void OnStarted()
            {
            }

            public void OnStopped()
            {
            }

            public void OnConnected(NetworkConnection connection)
            {
            }

            public void OnRejected(NetworkConnection connection)
            {
            }

            public void OnHandshake(NetworkConnection connection, PeerListenerHandshake handshake)
            {
                loop.Handle(connection, handshake);
                connection.Send(new NetworkOutgoingMessageBytes(new byte[4]));
            }

            public void OnException(NetworkConnection connection, Exception ex)
            {
            }

            public void OnDisconnected(NetworkConnection connection)
            {
            }
        }

        public class ConnectorCallback : PeerConnectorCallback
        {
            private readonly ConnectionLoop loop;

            public ConnectorCallback(ConnectionLoop loop)
            {
                this.loop = loop;
            }

            public void OnConnected(NetworkConnection connection)
            {
            }

            public void OnRejected(NetworkConnection connection)
            {
            }

            public void OnHandshake(NetworkConnection connection, PeerConnectorHandshake handshake)
            {
                loop.Handle(connection, handshake);
                connection.Send(new NetworkOutgoingMessageBytes(new byte[4]));
            }

            public void OnException(NetworkConnection connection, Exception ex)
            {
            }

            public void OnDisconnected(NetworkConnection connection)
            {
            }
        }
    }
}