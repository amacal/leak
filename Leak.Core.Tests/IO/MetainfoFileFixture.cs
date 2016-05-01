using Leak.Core.Tests.Resources;
using System.Collections.Generic;

namespace Leak.Core.Tests.IO
{
    public static class MetainfoFileFixture
    {
        public static IEnumerable<MetainfoFileTrackerCase> Trackers()
        {
            yield return new MetainfoFileTrackerCase
            {
                Name = "ubuntu",
                Torrent = Files.Ubuntu,
                Trackers = new[]
                {
                    "http://torrent.ubuntu.com:6969/announce",
                    "http://ipv6.torrent.ubuntu.com:6969/announce"
                }
            };

            yield return new MetainfoFileTrackerCase
            {
                Name = "neural-networks",
                Torrent = Files.NeuralNetworks,
                Trackers = new[]
                {
                    "http://academictorrents.com/announce.php?passkey=ad7e634a11458c90848ffb74ca666aa5",
                    "udp://tracker.openbittorrent.com:80/announce",
                    "udp://tracker.publicbt.com:80/announce"
                }
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

        public static IEnumerable<MetainfoFileHashCase> Hashes()
        {
            yield return new MetainfoFileHashCase
            {
                Name = "ubuntu",
                Torrent = Files.Ubuntu,
                Hash = ToBytes("3f19b149f53a50e14fc0b79926a391896eabab6f")
            };

            yield return new MetainfoFileHashCase
            {
                Name = "neural-networks",
                Torrent = Files.NeuralNetworks,
                Hash = ToBytes("e046bca3bc837053d1609ef33d623ee5c5af7300")
            };
        }

        public static IEnumerable<MetainfoFilePieceCase> Pieces()
        {
            yield return new MetainfoFilePieceCase
            {
                Name = "ubuntu",
                Torrent = Files.Ubuntu,
                Size = 512 * 1024,
                Count = 2248
            };

            yield return new MetainfoFilePieceCase
            {
                Name = "neural-networks",
                Torrent = Files.NeuralNetworks,
                Size = 1024 * 1024,
                Count = 6295
            };
        }

        private static byte[] ToBytes(string value)
        {
            byte[] result = new byte[value.Length / 2];

            for (int i = 0; i < value.Length; i += 2)
            {
                result[i / 2] = (byte)(ToByte(value[i]) * 16 + ToByte(value[i + 1]));
            }

            return result;
        }

        private static int ToByte(char value)
        {
            if (value >= '0' && value <= '9')
                return value - '0';

            if (value >= 'a' && value <= 'f')
                return value - 'a' + 10;

            if (value >= 'A' && value <= 'F')
                return value - 'a' + 10;

            return 0;
        }
    }
}