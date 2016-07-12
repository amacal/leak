using F2F.Sandbox;
using FluentAssertions;
using Leak.Core.IO;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Leak.Core.Tests
{
    [TestFixture]
    public class LeakMetadataTests
    {
        private int port;
        private FileSandbox sandbox;

        private EndpointResource active;
        private EndpointResource passive;

        [SetUp]
        public void SetUp()
        {
            port = 12345;
            sandbox = new FileSandbox(new EmptyFileLocator());

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
            sandbox.Dispose();
        }

        [Test]
        [TestCaseSource("GetMetainfoFixtures")]
        public void ShouldTransferMetadata(MetainfoFixture fixture)
        {
            passive.Metainfo = fixture.Factory.Invoke(sandbox);

            passive.Client = new LeakClient(with =>
            {
                with.Listener(listener =>
                {
                    listener.Port(port);
                });

                with.Extensions(extensions =>
                {
                    extensions.Metadata();
                });

                with.Torrents(torrents =>
                {
                    torrents.Schedule(schedule =>
                    {
                        schedule.Metainfo(passive.Metainfo);
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
                    torrents.Schedule(schedule =>
                    {
                        schedule.Hash(passive.Metainfo.Hash);
                        schedule.DownloadMetadata();
                    });

                    torrents.Peer(peer =>
                    {
                        peer.Remote("127.0.0.1", port);
                    });
                });

                with.Callback(callback =>
                {
                    callback.MetadataDownloaded(metadata =>
                    {
                        active.Metainfo = metadata.Metainfo;
                        active.Synchronizer.Set();
                    });
                });
            });

            passive.Client.Start();
            active.Client.Start();

            WaitHandle[] handles = { active.Synchronizer.WaitHandle };
            TimeSpan delay = TimeSpan.FromSeconds(1);

            WaitHandle.WaitAll(handles, delay).Should().BeTrue();

            passive.Metainfo.Data.Should().Equal(active.Metainfo.Data);
        }

        public static IEnumerable<MetainfoFixture> GetMetainfoFixtures()
        {
            yield return new MetainfoFixture
            {
                Name = "1kB",
                Factory = sandbox =>
                {
                    string path = sandbox.CreateFile("binary.data");
                    File.WriteAllBytes(path, Bytes.Random(1024));

                    return new MetainfoFile(with =>
                    {
                        with.Include(sandbox.Directory);
                    });
                }
            };

            yield return new MetainfoFixture
            {
                Name = "15MB",
                Factory = sandbox =>
                {
                    string path = sandbox.CreateFile("binary.data");
                    File.WriteAllBytes(path, Bytes.Random(15 * 1024 * 1024));

                    return new MetainfoFile(with =>
                    {
                        with.Include(sandbox.Directory);
                    });
                }
            };
        }

        public class MetainfoFixture
        {
            public string Name;

            public Func<FileSandbox, MetainfoFile> Factory;

            public override string ToString()
            {
                return Name;
            }
        }

        private class EndpointResource : IDisposable
        {
            public LeakClient Client;
            public ManualResetEventSlim Synchronizer;

            public MetainfoFile Metainfo;

            public void Dispose()
            {
                Client?.Dispose();
                Synchronizer?.Dispose();
            }
        }
    }
}