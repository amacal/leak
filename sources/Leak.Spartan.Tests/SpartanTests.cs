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
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerTaskCompletedWhenReceivedMetadataDiscovered()
        {
            using (SpartanFixture fixture = new SpartanFixture())
            using (SpartanSession session = fixture.Start(Goal.Discover))
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnTaskCompleted, data =>
                {
                    return data.Task == Goal.Discover;
                });

                session.Spartan.Start();
                session.Stage.Discovering.Wait(5000).Should().BeTrue();

                session.Spartan.Handle(new MetadataDiscovered
                {
                    Hash = session.Hash,
                    Metainfo = null
                });

                handler.Wait().Should().BeTrue();
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
                session.Stage.Discovering.Wait(5000).Should().BeTrue();

                session.Spartan.Handle(discovered);
                session.Repository.Handle(discovered);

                handler.Wait().Should().BeTrue();
            }
        }
    }
}