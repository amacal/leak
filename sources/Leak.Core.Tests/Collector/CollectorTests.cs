using FluentAssertions;
using Leak.Core.Collector;
using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Listener;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Leak.Core.Tests.Collector
{
    [TestFixture]
    public class CollectorTests
    {
        [Test]
        public void CanCollectListenerConnections()
        {
            FileHash hash = new FileHash(Bytes.Random(20));
            PeerHash peer = new PeerHash(Bytes.Random(20));
            CollectorContext context = new CollectorContext();

            PeerCollector collector = new PeerCollector(with =>
            {
                with.Callback = context;
            });

            PeerListener listener = new PeerListener(with =>
            {
                with.Port = 8080;
                with.Callback = collector.CreateListenerCallback();
                with.Hashes = new FileHashCollection(hash);
            });

            PeerConnector connector = new PeerConnector(with =>
            {
                with.Hash = hash;
                with.Peer = peer;
            });

            listener.Start();
            connector.ConnectTo("localhost", 8080);

            context.Ready.WaitOne(TimeSpan.FromSeconds(2));
            context.Connected.Should().Contain(peer);
        }

        public class CollectorContext : PeerCollectorCallbackBase
        {
            private readonly HashSet<PeerHash> connected;
            private readonly ManualResetEvent onReady;

            public CollectorContext()
            {
                this.connected = new HashSet<PeerHash>();
                this.onReady = new ManualResetEvent(false);
            }

            public WaitHandle Ready
            {
                get { return onReady; }
            }

            public IEnumerable<PeerHash> Connected
            {
                get { return connected; }
            }

            public override void OnHandshake(PeerEndpoint endpoint)
            {
                connected.Add(endpoint.Peer);
                onReady.Set();
            }

            public override void OnDisconnected(PeerHash peer)
            {
                connected.Remove(peer);
            }
        }
    }
}