using FluentAssertions;
using Leak.Core.Common;
using Leak.Core.Listener;
using Leak.Core.Tests.Stubs;
using NUnit.Framework;
using System;

namespace Leak.Core.Tests.Listener
{
    [TestFixture]
    public class ListenerTests
    {
        [Test]
        public void CanAcceptNewConnection()
        {
            FileHash hash = new FileHash(Bytes.Random(20));

            using (NetworkContainer container = new NetworkContainer())
            {
                PeerListener listener = new PeerListener(with =>
                {
                    with.Port = 8080;
                    with.Hashes = new FileHashCollection(hash);
                });

                listener.Start();
                container.Active("active");
                container.ConnectTo("active", "localhost", 8080);

                HandshakeNegotiatorActiveStub active = new HandshakeNegotiatorActiveStub(container.Pool, container["active"], with =>
                {
                    with.Hash = hash;
                });

                active.Execute();
                active.Context.Ready.WaitOne(TimeSpan.FromSeconds(2));
                active.Context.Handshake.Hash.ToBytes().Should().Equal(hash.ToBytes());
            }
        }
    }
}