using Leak.Core.IO;
using Leak.Core.Tests.Resources;
using NUnit.Framework;
using System.Linq;

namespace Leak.Core.Tests.IO
{
    [TestFixture]
    public class TorrentDirectoryTests
    {
        [Test]
        public void Should()
        {
            MetainfoFile metainfo = new MetainfoFile(Files.NeuralNetworks);
            TorrentDirectory directory = new TorrentDirectory(metainfo, "");

            TorrentPiece[] pieces = directory.Pieces.ToArray();
        }
    }
}