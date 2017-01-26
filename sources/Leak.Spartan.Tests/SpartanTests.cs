using FakeItEasy;
using FluentAssertions;
using Leak.Common;
using Leak.Events;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Spartan.Tests
{
    public class SpartanTests
    {
        [Test]
        public void ShouldTriggerTaskStartedWithDiscovery()
        {
            using (SpartanFixture fixture = new SpartanFixture())
            using (SpartanSession session = fixture.Start(Goal.Discover))
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnTaskStarted, data =>
                {
                    return data.Task == Goal.Discover;
                });

                session.Spartan.Start();
                session.Pipeline.Process();

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldStartMetagetServiceWhenDiscovering()
        {
            using (SpartanFixture fixture = new SpartanFixture())
            using (SpartanSession session = fixture.Start(Goal.Discover))
            {
                session.Spartan.Start();
                session.Pipeline.Process();

                A.CallTo(() => session.Metaget.Start()).MustHaveHappened();
            }
        }

        [Test]
        public void ShouldStartMetashareServiceWhenDiscovering()
        {
            using (SpartanFixture fixture = new SpartanFixture())
            using (SpartanSession session = fixture.Start(Goal.Discover))
            {
                session.Spartan.Start();
                session.Pipeline.Process();

                A.CallTo(() => session.Metashare.Start()).MustHaveHappened();
            }
        }

        [Test]
        public void ShouldTriggerTaskCompletedWhenReceivedMetadataDiscovered()
        {
            using (SpartanFixture fixture = new SpartanFixture())
            using (SpartanSession session = fixture.Start(Goal.Discover))
            {
                MetadataDiscovered discovered = new MetadataDiscovered
                {
                    Hash = session.Hash,
                    Metainfo = session.Metainfo
                };

                Trigger handler = Trigger.Bind(ref session.Hooks.OnTaskCompleted, data =>
                {
                    return data.Task == Goal.Discover;
                });

                session.Spartan.Start();
                session.Pipeline.Process();

                session.Stage.Discovering.Wait(5000).Should().BeTrue();

                session.Spartan.Handle(discovered);
                session.Pipeline.Process();

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldStopMetagetWhenDiscoveringCompleted()
        {
            using (SpartanFixture fixture = new SpartanFixture())
            using (SpartanSession session = fixture.Start(Goal.Discover))
            {
                MetadataDiscovered discovered = new MetadataDiscovered
                {
                    Hash = session.Hash,
                    Metainfo = session.Metainfo
                };

                session.Spartan.Start();
                session.Pipeline.Process();

                session.Stage.Discovering.Wait(5000).Should().BeTrue();

                session.Spartan.Handle(discovered);
                session.Pipeline.Process();

                session.Stage.Discovered.Wait(5000).Should().BeTrue();
                A.CallTo(() => session.Metaget.Stop()).MustHaveHappened();
            }
        }

        [Test]
        public void ShouldTriggerTaskCompletedWhenVerified()
        {
            using (SpartanFixture fixture = new SpartanFixture())
            using (SpartanSession session = fixture.Start(Goal.Discover | Goal.Verify))
            {
                MetadataDiscovered discovered = new MetadataDiscovered
                {
                    Hash = session.Hash,
                    Metainfo = session.Metainfo
                };

                Trigger handler = Trigger.Bind(ref session.Hooks.OnTaskStarted, data =>
                {
                    return data.Task == Goal.Verify;
                });

                session.Spartan.Start();
                session.Pipeline.Process();

                session.Stage.Discovering.Wait(5000).Should().BeTrue();

                session.Spartan.Handle(discovered);
                session.Pipeline.Process();

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldVerifyUsingRepository()
        {
            using (SpartanFixture fixture = new SpartanFixture())
            using (SpartanSession session = fixture.Start(Goal.Discover | Goal.Verify))
            {
                MetadataDiscovered discovered = new MetadataDiscovered
                {
                    Hash = session.Hash,
                    Metainfo = session.Metainfo
                };

                session.Spartan.Start();
                session.Pipeline.Process();

                session.Stage.Discovering.Wait(5000).Should().BeTrue();

                session.Spartan.Handle(discovered);
                session.Pipeline.Process();

                A.CallTo(() => session.Repository.Verify(A<Bitfield>.Ignored)).MustHaveHappened();
            }
        }

        [Test]
        public void ShouldStartRetrieverWhen()
        {
            using (SpartanFixture fixture = new SpartanFixture())
            using (SpartanSession session = fixture.Start(Goal.Discover | Goal.Verify | Goal.Download))
            {
                MetadataDiscovered discovered = new MetadataDiscovered
                {
                    Hash = session.Hash,
                    Metainfo = session.Metainfo
                };

                DataVerified verified = new DataVerified
                {
                    Hash = session.Hash,
                    Bitfield = new Bitfield(session.Metainfo.Pieces.Length)
                };

                session.Spartan.Start();
                session.Pipeline.Process();

                session.Stage.Discovering.Wait(5000).Should().BeTrue();

                session.Spartan.Handle(discovered);
                session.Pipeline.Process();

                session.Stage.Verifying.Wait(5000).Should().BeTrue();

                session.Spartan.Handle(verified);
                session.Pipeline.Process();

                session.Stage.Downloading.Wait(5000).Should().BeTrue();
                A.CallTo(() => session.Retriever.Start()).MustHaveHappened();
            }
        }
    }
}