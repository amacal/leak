using Leak.Core.Tests.Resources;
using System.Collections.Generic;

namespace Leak.Core.Tests.IO
{
    public static class MetainfoFileFixture
    {
        public static byte[] Ubuntu
        {
            get { return Files.Ubuntu; }
        }

        public static byte[] NeuralNetworks
        {
            get { return Files.NeuralNetworks; }
        }

        public static IEnumerable<MetainfoFileTrackerCase> Trackers()
        {
            yield return new MetainfoFileTrackerCase
            {
                Name = "ubuntu",
                Torrent = Files.Ubuntu,
                Trackers = new[] { "http://torrent.ubuntu.com:6969/announce" }
            };

            yield return new MetainfoFileTrackerCase
            {
                Name = "neural-networks",
                Torrent = Files.NeuralNetworks,
                Trackers = new[] { "http://academictorrents.com/announce.php?passkey=ad7e634a11458c90848ffb74ca666aa5" }
            };
        }

        public static IEnumerable<MetainfoFileNameCase> Names()
        {
            yield return new MetainfoFileNameCase
            {
                Name = "ubuntu",
                Torrent = Files.Ubuntu,
                Names = new[] { "ubuntu-15.10-desktop-amd64.iso" }
            };

            yield return new MetainfoFileNameCase
            {
                Name = "neural-networks",
                Torrent = Files.NeuralNetworks,
                Names = new[]
                {
                    "Neural networks [2.5]  - Training neural networks - activation function derivative-tf9p1xQbWNM.mp4",
                    "Neural networks [3.10]  - Conditional random fields - belief propagation--z5lKPHcumo.mp4"
                }
            };
        }

        public static IEnumerable<MetainfoFileCountCase> Counts()
        {
            yield return new MetainfoFileCountCase
            {
                Name = "ubuntu",
                Torrent = Files.Ubuntu,
                Count = 1
            };

            yield return new MetainfoFileCountCase
            {
                Name = "neural-networks",
                Torrent = Files.NeuralNetworks,
                Count = 92
            };
        }
    }
}