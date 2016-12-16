using FluentAssertions;
using Leak.Common;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Connector.Tests
{
    public class ConnectorTests
    {
        [Test]
        public void ShouldTriggerConnectionEstablished()
        {
            using (ConnectorFixture fixture = new ConnectorFixture())
            using (ConnectorSession session = fixture.Start())
            {
                FileHash hash = FileHash.Random();
                PeerAddress address = PeerAddress.Parse(session.Endpoint);

                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnConnectionEstablished, data =>
                {
                    data.Remote.Should().NotBeNull();
                    data.Remote.Host.Should().Be("127.0.0.1");
                    data.Remote.Port.Should().Be(session.Endpoint.Port);
                });

                session.Server.Accept(null);
                session.Connector.ConnectTo(hash, address);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerConnectionRejected()
        {
            using (ConnectorFixture fixture = new ConnectorFixture())
            using (ConnectorSession session = fixture.Start())
            {
                FileHash hash = FileHash.Random();
                PeerAddress address = PeerAddress.Parse(session.Endpoint);

                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnConnectionEstablished, data =>
                {
                    data.Remote.Should().NotBeNull();
                    data.Remote.Host.Should().Be("127.0.0.1");
                    data.Remote.Port.Should().Be(session.Endpoint.Port);
                });

                session.Connector.ConnectTo(hash, address);
                handler.Wait().Should().BeTrue();
            }
        }
    }
}