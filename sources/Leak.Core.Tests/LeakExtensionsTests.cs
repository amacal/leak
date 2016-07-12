using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading;

namespace Leak.Core.Tests
{
    [TestFixture]
    public class LeakExtensionsTests
    {
        private int port;
        private byte[] hash;

        private EndpointResource active;
        private EndpointResource passive;

        [SetUp]
        public void SetUp()
        {
            port = 12345;
            hash = Bytes.Random(20);

            active = new EndpointResource();
            active.Synchronizer = new ManualResetEventSlim(false);

            passive = new EndpointResource();
            passive.Synchronizer = new ManualResetEventSlim(false);
        }

        [TearDown]
        public void TearDown()
        {
            active.Dispose();
            passive.Dispose();
        }

        [Test]
        public void ShouldDiscoverMetadataExtension()
        {
            passive.Client = new LeakClient(with =>
            {
                with.Listener(listener =>
                {
                    listener.Port(port);
                    listener.OnlyEncrypted();
                });

                with.Extensions(extensions =>
                {
                    extensions.Metadata();
                });

                with.Torrents(torrents =>
                {
                    torrents.Schedule(hash);
                });

                with.Callback(callback =>
                {
                    callback.HandshakeExchanged(handshake =>
                    {
                        passive.SupportsExtensions = handshake.SupportsExtensions;
                    });

                    callback.ExtensionsExchanged(extensions =>
                    {
                        passive.SupportsMetadata = extensions.SupportsMetadata;
                        passive.Synchronizer.Set();
                    });
                });
            });

            active.Client = new LeakClient(with =>
            {
                with.Extensions(extensions =>
                {
                    extensions.Metadata();
                });

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
                    callback.HandshakeExchanged(handshake =>
                    {
                        active.SupportsExtensions = handshake.SupportsExtensions;
                    });

                    callback.ExtensionsExchanged(extensions =>
                    {
                        active.SupportsMetadata = extensions.SupportsMetadata;
                        active.Synchronizer.Set();
                    });
                });
            });

            passive.Client.Start();
            active.Client.Start();

            WaitHandle[] handles = { passive.Synchronizer.WaitHandle, active.Synchronizer.WaitHandle };
            TimeSpan delay = TimeSpan.FromSeconds(1);

            WaitHandle.WaitAll(handles, delay).Should().BeTrue();

            active.SupportsExtensions.Should().BeTrue();
            active.SupportsMetadata.Should().BeTrue();

            passive.SupportsExtensions.Should().BeTrue();
            passive.SupportsMetadata.Should().BeTrue();
        }

        private class EndpointResource : IDisposable
        {
            public LeakClient Client;
            public ManualResetEventSlim Synchronizer;

            public bool SupportsExtensions;
            public bool SupportsMetadata;

            public void Dispose()
            {
                Client?.Dispose();
                Synchronizer?.Dispose();
            }
        }
    }
}