using FluentAssertions;
using Leak.Core.Encoding;
using NUnit.Framework;

namespace Leak.Core.Tests.Encoding
{
    [TestFixture]
    public class BencoderTests
    {
        [Test]
        public void ShouldReturnNotNullValue()
        {
            byte[] data = BencoderFixture.Ubuntu1510DesktopAmd64Iso;

            BencodedValue value = Bencoder.Decode(data);

            value.Should().NotBeNull();
        }

        [Test]
        public void ShouldContainAllKeys()
        {
            byte[] data = BencoderFixture.Ubuntu1510DesktopAmd64Iso;
            string[] keys = { "info", "announce", "piece length", "pieces", "name", "length" };

            BencodedValue value = Bencoder.Decode(data);
            string[] result = value.AllKeys();

            keys.Should().BeSubsetOf(result);
        }

        [Test]
        public void ShouldContainAllTexts()
        {
            byte[] data = BencoderFixture.Ubuntu1510DesktopAmd64Iso;
            string[] texts = { "http://torrent.ubuntu.com:6969/announce", "ubuntu-15.10-desktop-amd64.iso" };

            BencodedValue value = Bencoder.Decode(data);
            string[] result = value.AllTexts();

            texts.Should().BeSubsetOf(result);
        }
    }
}