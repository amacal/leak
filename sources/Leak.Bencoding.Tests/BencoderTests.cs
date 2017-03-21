using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Leak.Bencoding.Tests
{
    public class BencoderTests
    {
        [Test]
        public void ShouldDecodeAsInteger()
        {
            byte[] data = Encoding.ASCII.GetBytes("i10e");
            BencodedValue value = Bencoder.Decode(data);

            value.Should().NotBeNull();
            value.Number.Should().NotBeNull();
            value.Number.ToInt32().Should().Be(10);
        }
    }
}