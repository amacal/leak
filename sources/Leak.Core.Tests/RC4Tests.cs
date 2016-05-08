using FluentAssertions;
using NUnit.Framework;

namespace Leak.Core.Tests
{
    [TestFixture]
    public class RC4Tests
    {
        [Test]
        public void CanDecryptEncryptedMessage()
        {
            byte[] key = Bytes.Random(20);
            byte[] message = Bytes.Parse("0102030405");

            RC4 encryptor = new RC4(key, 0);
            RC4 decryptor = new RC4(key, 0);

            byte[] encrypted = encryptor.Encrypt(message);
            byte[] decrypted = decryptor.Decrypt(encrypted);

            decrypted.Should().BeEquivalentTo(message);
        }

        [Test]
        public void CanDecryptEncryptedMessageFirstPart()
        {
            byte[] key = Bytes.Random(20);
            byte[] message = Bytes.Parse("0102030405");

            RC4 encryptor = new RC4(key, 0);
            RC4 decryptor = new RC4(key, 0);

            byte[] encrypted = encryptor.Encrypt(message.ToBytes(0, 3));
            byte[] decrypted = decryptor.Decrypt(encrypted);

            decrypted.Should().BeEquivalentTo(Bytes.Parse("010203"));
        }

        [Test]
        public void CanDecryptEncryptedMessageSecondPart()
        {
            byte[] key = Bytes.Random(20);
            byte[] message = Bytes.Parse("0102030405");

            RC4 encryptor = new RC4(key, 0);
            RC4 decryptor = new RC4(key, 0);

            byte[] encrypted = encryptor.Encrypt(message.ToBytes(3, 2));
            byte[] decrypted = decryptor.Decrypt(encrypted);

            decrypted.Should().BeEquivalentTo(Bytes.Parse("0405"));
        }

        [Test]
        public void CanDecryptMessageTwice()
        {
            byte[] key = Bytes.Random(20);
            byte[] message = Bytes.Parse("0102030405");

            RC4 encryptor = new RC4(key, 0);
            RC4 decryptor = new RC4(key, 0);

            byte[] encrypted = encryptor.Encrypt(message);
            byte[] first = decryptor.Clone().Decrypt(encrypted);
            byte[] second = decryptor.Clone().Decrypt(encrypted);

            first.Should().BeEquivalentTo(second);
        }
    }
}