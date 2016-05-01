using FluentAssertions;
using Leak.Core.Net;
using NUnit.Framework;

namespace Leak.Core.Tests.Net
{
    [TestFixture]
    public class PeerCryptographyTests
    {
        [Test]
        public void ShouldGenerate768bitPublicKey()
        {
            PeerCredentials credentials = PeerCryptography.Generate();

            credentials.PublicKey.Should().HaveCount(768 / 8);
        }

        [Test]
        public void ShouldGeneratePaddingBetween0And512BytesLength()
        {
            PeerCredentials credentials = PeerCryptography.Generate();

            credentials.Padding.Length.Should().BeInRange(0, 512);
        }

        [Test]
        public void ShouldReturnEqualSecretForBothPeers()
        {
            PeerCredentials credentials1 = PeerCryptography.Generate();
            PeerCredentials credentials2 = PeerCryptography.Generate();

            byte[] secret1 = PeerCryptography.Secret(credentials1, credentials2.PublicKey);
            byte[] secret2 = PeerCryptography.Secret(credentials2, credentials1.PublicKey);

            secret1.Should().BeEquivalentTo(secret2);
        }
    }
}