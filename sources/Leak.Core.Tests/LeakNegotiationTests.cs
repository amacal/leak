using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading;

namespace Leak.Core.Tests
{
    [TestFixture]
    public class LeakNegotiationTests
    {
        private int port;
        private byte[] hash;

        private LeakClient activeClient;
        private LeakClient passiveClient;

        private ManualResetEventSlim activeHandle;
        private ManualResetEventSlim passiveHandle;

        [SetUp]
        public void SetUp()
        {
            port = 12345;
            hash = Bytes.Random(20);

            activeHandle = new ManualResetEventSlim(false);
            passiveHandle = new ManualResetEventSlim(false);
        }

        [TearDown]
        public void TearDown()
        {
            activeClient?.Dispose();
            passiveClient?.Dispose();
        }

        [Test]
        public void ShouldEstablishEncryptedConnection()
        {
            passiveClient = new LeakClient(with =>
            {
                with.Listener(listener =>
                {
                    listener.Port(port);
                    listener.OnlyEncrypted();
                });

                with.Torrents(torrents =>
                {
                    torrents.Schedule(hash);
                });

                with.Callback(callback =>
                {
                    callback.HandshakeExchanged(connection =>
                    {
                        passiveHandle.Set();
                    });
                });
            });

            activeClient = new LeakClient(with =>
            {
                with.Torrents(torrents =>
                {
                    torrents.Schedule(hash);
                    torrents.Peer(peer =>
                    {
                        peer.Encrypted();
                        peer.Remote("127.0.0.1", port);
                    });
                });

                with.Callback(callback =>
                {
                    callback.HandshakeExchanged(connection =>
                    {
                        activeHandle.Set();
                    });
                });
            });

            passiveClient.Start();
            activeClient.Start();

            WaitHandle[] handles = { passiveHandle.WaitHandle, activeHandle.WaitHandle };
            TimeSpan delay = TimeSpan.FromSeconds(1);

            WaitHandle.WaitAll(handles, delay).Should().BeTrue();
        }
    }
}