using FluentAssertions;
using Leak.Core.IO;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Tests.IO
{
    [TestFixture]
    public class MetainfoFileTests
    {
        [Test]
        [TestCaseSource(typeof(MetainfoFileFixture), "Trackers")]
        public void ShouldGetAllTrackers(MetainfoFileTrackerCase source)
        {
            MetainfoFile file = new MetainfoFile(source.Torrent);

            source.Trackers.Should().BeSubsetOf(file.Trackers);
        }

        [Test]
        [TestCaseSource(typeof(MetainfoFileFixture), "Names")]
        public void ShouldGetAllNames(MetainfoFileNameCase source)
        {
            MetainfoFile file = new MetainfoFile(source.Torrent);
            IEnumerable<string> names = file.Entries.Select(x => x.Name);

            source.Names.Should().BeSubsetOf(names);
        }

        [Test]
        [TestCaseSource(typeof(MetainfoFileFixture), "Counts")]
        public void ShouldHaveAllFiles(MetainfoFileCountCase source)
        {
            MetainfoFile file = new MetainfoFile(source.Torrent);

            file.Entries.Should().HaveCount(source.Count);
        }
    }
}