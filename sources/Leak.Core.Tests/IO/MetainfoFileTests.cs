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
        public void ShouldHaveSomeTrackers(MetainfoFileTrackerCase source)
        {
            MetainfoFile file = new MetainfoFile(source.Torrent);
            IEnumerable<string> trackers = file.Trackers.Select(x => x.Uri.ToString());

            source.Trackers.Should().BeSubsetOf(trackers);
        }

        [Test]
        [TestCaseSource(typeof(MetainfoFileFixture), "Names")]
        public void ShouldHaveSomeNames(MetainfoFileNameCase source)
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

        [Test]
        [TestCaseSource(typeof(MetainfoFileFixture), "Hashes")]
        public void ShouldHaveRightHashes(MetainfoFileHashCase source)
        {
            MetainfoFile file = new MetainfoFile(source.Torrent);

            file.Hash.Should().Equal(source.Hash);
        }

        [Test]
        [TestCaseSource(typeof(MetainfoFileFixture), "Pieces")]
        public void ShouldHaveRightPieces(MetainfoFilePieceCase source)
        {
            MetainfoFile file = new MetainfoFile(source.Torrent);

            file.Pieces.Size.Should().Equals(source.Size);
            file.Pieces.Count.Should().Be(source.Count);
            file.Pieces.Should().HaveCount(source.Count);
        }
    }
}