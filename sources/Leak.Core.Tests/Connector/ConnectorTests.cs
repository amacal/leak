using FluentAssertions;
using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Network;
using Leak.Core.Tests.Stubs;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Leak.Core.Tests.Connector
{
    [TestFixture]
    public class ConnectorTests
    {
        [Test]
        public async Task CanEstablishNewConnection()
        {
            FileHash hash = new FileHash(Bytes.Random(20));

            using (NetworkContainer container = new NetworkContainer())
            {
                container.Passive("passive");
                Task<NetworkConnection> connection = container.Listen("passive", 8080);

                PeerConnector connector = new PeerConnector(with =>
                {
                });

                connector.ConnectTo(hash, new PeerAddress("localhost", 8080));

                HandshakeNegotiatorPassiveStub passive = new HandshakeNegotiatorPassiveStub(container.Pool, await connection, with =>
                {
                    with.Hashes.Add(hash);
                });

                passive.Execute();
                passive.Context.Ready.WaitOne(TimeSpan.FromSeconds(200));
                passive.Context.Handshake.Hash.ToBytes().Should().Equal(hash.ToBytes());
            }
        }
    }
}