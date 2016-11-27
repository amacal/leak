using FluentAssertions;
using Leak.Core.Common;
using Leak.Core.Events;
using Leak.Core.Metadata;
using Leak.Core.Retriever;
using Leak.Core.Tests.Core;
using NUnit.Framework;

namespace Leak.Core.Tests.Components
{
    public class RetrieverTests
    {
        private RetrieverHooks hooks;
        private RetrieverConfiguration configuration;

        private Fixture fixture;
        private Environment environemnt;

        [OneTimeSetUp]
        public void OneSetUp()
        {
            fixture = new Fixture();
        }

        [OneTimeTearDown]
        public void OneTearDown()
        {
            fixture.Dispose();
            fixture = null;
        }

        [SetUp]
        public void SetUp()
        {
            environemnt = new Environment(fixture.Debian.Metadata);

            configuration = new RetrieverConfiguration();
            hooks = new RetrieverHooks();
        }

        [TearDown]
        public void TearDown()
        {
            environemnt.Dispose();
            environemnt = null;
        }

        private RetrieverService NewRetrieverService()
        {
            Metainfo metainfo = fixture.Debian.Metadata.Metainfo;
            Bitfield bitfield = new Bitfield(metainfo.Pieces.Length);

            return new RetrieverService(metainfo, environemnt.Destination, bitfield, environemnt.Files, environemnt.Pipeline, hooks, configuration);
        }

        [Test]
        public void ShouldTriggerDataChanged()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnDataChanged, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Hash);
                data.Completed.Should().Be(1);
            });

            using (RetrieverService retriever = NewRetrieverService())
            {
                retriever.Start();
                retriever.HandleDataReceived(fixture.Debian.Events.DataReceived[0]);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerOnDataCompleted()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnDataCompleted, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Hash);
            });

            using (RetrieverService retriever = NewRetrieverService())
            {
                retriever.Start();

                foreach (DataReceived data in fixture.Debian.Events.DataReceived)
                {
                    retriever.HandleDataReceived(data);
                }

                handler.Wait().Should().BeTrue();
            }
        }
    }
}