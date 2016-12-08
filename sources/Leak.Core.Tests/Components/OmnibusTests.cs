using FluentAssertions;
using Leak.Common;
using Leak.Core.Core;
using Leak.Core.Metadata;
using Leak.Core.Omnibus;
using Leak.Core.Tests.Core;
using Leak.Events;
using NUnit.Framework;

namespace Leak.Core.Tests.Components
{
    public class OmnibusTests
    {
        private OmnibusHooks hooks;
        private OmnibusConfiguration configuration;

        private Fixture fixture;
        private LeakPipeline pipeline;

        [OneTimeSetUp]
        public void OneSetUp()
        {
            fixture = new Fixture();

            pipeline = new LeakPipeline();
            pipeline.Start();
        }

        [OneTimeTearDown]
        public void OneTearDown()
        {
            pipeline.Stop();
            pipeline = null;

            fixture.Dispose();
            fixture = null;
        }

        [SetUp]
        public void SetUp()
        {
            hooks = new OmnibusHooks();
            configuration = new OmnibusConfiguration();
        }

        private OmnibusService NewOmnibusService()
        {
            Metainfo metainfo = fixture.Debian.Metadata.Metainfo;
            Bitfield bitfield = new Bitfield(metainfo.Pieces.Length);

            return new OmnibusService(metainfo, bitfield, pipeline, hooks, configuration);
        }

        [Test]
        public void ShouldTriggerDataChangedWhenPieceCompleted()
        {
            Trigger trigger = Trigger.Bind(ref hooks.OnDataChanged, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Metainfo.Hash);
                data.Completed.Should().Be(1);
            });

            using (OmnibusService service = NewOmnibusService())
            {
                service.Start();
                service.Complete(1);

                trigger.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerDataCompletedWhenAllPiecesCompleted()
        {
            Trigger trigger = Trigger.Bind(ref hooks.OnDataCompleted, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Metainfo.Hash);
            });

            using (OmnibusService service = NewOmnibusService())
            {
                service.Start();

                for (int i = 0; i < fixture.Debian.Metadata.Metainfo.Pieces.Length; i++)
                {
                    service.Complete(i);
                }

                trigger.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerPieceReadyWhenAllBlocksCompleted()
        {
            Trigger trigger = Trigger.Bind(ref hooks.OnPieceReady, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Metainfo.Hash);
                data.Piece.Should().Be(1);
            });

            using (OmnibusService service = NewOmnibusService())
            {
                service.Start();
                service.Complete(new OmnibusBlock(1, 0, 16384));

                trigger.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerPieceCompletedWhenAllBlocksCompleted()
        {
            Trigger trigger = Trigger.Bind(ref hooks.OnPieceCompleted, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Metainfo.Hash);
                data.Piece.Should().Be(1);
            });

            using (OmnibusService service = NewOmnibusService())
            {
                service.Start();
                service.Complete(new OmnibusBlock(1, 0, 16384));
                service.Complete(1);

                trigger.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerBlockReserved()
        {
            PeerChanged changed = new PeerChanged
            {
                Peer = PeerHash.Random(),
                Bitfield = Bitfield.Sequence(false, true, false),
                IsLocalInterestedInRemote = true,
                IsRemoteChokingLocal = false
            };

            Trigger trigger = Trigger.Bind(ref hooks.OnBlockReserved, data =>
             {
                 data.Hash.Should().Be(fixture.Debian.Metadata.Metainfo.Hash);
                 data.Peer.Should().Be(changed.Peer);
                 data.Piece.Should().Be(1);
                 data.Block.Should().Be(0);
             });

            using (OmnibusService service = NewOmnibusService())
            {
                service.Start();
                service.Handle(changed);
                service.Schedule(OmnibusStrategy.RarestFirst, changed.Peer, 1);

                trigger.Wait().Should().BeTrue();
            }
        }
    }
}