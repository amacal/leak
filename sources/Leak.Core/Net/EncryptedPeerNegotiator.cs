using System;

namespace Leak.Core.Net
{
    public class EncryptedPeerNegotiator : PeerNegotiator
    {
        private readonly PeerHandshake handshake;
        private readonly PeerCredentials credentials;

        public EncryptedPeerNegotiator(PeerHandshake handshake)
        {
            this.handshake = handshake;
            this.credentials = PeerCryptography.Generate();
        }

        public override void Active(PeerNegotiatorAware channel)
        {
            channel.Send(new PeerKeyExchange(credentials));
            channel.Receive(96, keyExchangeMessage =>
            {
                PeerKeyExchange exchange = new PeerKeyExchange(keyExchangeMessage);
                byte[] secret = PeerCryptography.Secret(credentials, exchange.Key);

                RC4 initiatorKey = new RC4(Bytes.Hash("keyA", secret, handshake.Hash), 1024);
                RC4 receiverKey = new RC4(Bytes.Hash("keyB", secret, handshake.Hash), 1024);

                channel.Remove(96);
                channel.Send(new PeerCryptoHash(secret, handshake.Hash));
                channel.Send(new PeerCryptoPayload(), initiatorKey);
                channel.Send(new PeerCryptoHandshake(), initiatorKey);

                Func<PeerMessage, bool> hasCrypto = message =>
                {
                    RC4 decryptor = receiverKey.Clone();
                    byte[] synchronize = PeerCryptoPayload.GetVerification();

                    byte[] encrypted = synchronize.Encrypt(decryptor);
                    int offset = Find(message.ToBytes(), encrypted);

                    return offset >= 0 && message.Length >= offset + PeerCryptoPayload.MinimumSize;
                };

                channel.Receive(hasCrypto, cryptoMessageSync =>
                {
                    RC4 decryptor = receiverKey.Clone();
                    byte[] synchronize = PeerCryptoPayload.GetVerification();

                    byte[] encrypted = synchronize.Encrypt(decryptor);
                    int offset = Find(cryptoMessageSync.ToBytes(), encrypted);

                    channel.Remove(offset);

                    channel.Receive(PeerCryptoPayload.MinimumSize, cryptoMessagePeek =>
                    {
                        decryptor = receiverKey.Clone();
                        PeerMessage decrypted = cryptoMessagePeek.Decrypt(decryptor);
                        int size = PeerCryptoPayload.GetSize(decrypted);

                        channel.Receive(size, cryptoMessage =>
                        {
                            receiverKey.Skip(size);
                            channel.Remove(size);
                            channel.Send(handshake, initiatorKey);

                            channel.Receive(PeerHandshake.MinSize, handshakeMessagePeek =>
                            {
                                decryptor = receiverKey.Clone();
                                decrypted = handshakeMessagePeek.Decrypt(decryptor);
                                size = PeerHandshake.GetSize(decrypted);

                                channel.Receive(size, handshakeMessage =>
                                {
                                    receiverKey.Skip(size);
                                    channel.Remove(size);

                                    channel.Continue(handshake, x => x.Encrypt(initiatorKey), x => x.Decrypt(receiverKey.Clone()), (buffer, count) => { buffer.Remove(count); receiverKey.Skip(count); });
                                });
                            });
                        });
                    });
                });
            });
        }

        public override void Passive(PeerNegotiatorAware channel)
        {
            channel.Receive(96, keyExchangeMessage =>
            {
                PeerKeyExchange exchange = new PeerKeyExchange(keyExchangeMessage);
                byte[] secret = PeerCryptography.Secret(credentials, exchange.Key);

                RC4 initiatorKey = new RC4(Bytes.Hash("keyA", secret, handshake.Hash), 1024);
                RC4 receiverKey = new RC4(Bytes.Hash("keyB", secret, handshake.Hash), 1024);

                channel.Remove(96);
                channel.Send(new PeerKeyExchange(credentials));

                Func<PeerMessage, bool> hasHashes = message =>
                {
                    byte[] synchronize = PeerCryptoHash.GetHash(secret);
                    int offset = Find(message.ToBytes(), synchronize);

                    return offset >= 0 && message.Length >= offset + 40;
                };

                channel.Receive(hasHashes, hashMessage =>
                {
                    byte[] synchronize = PeerCryptoHash.GetHash(secret);
                    int offset = Find(hashMessage.ToBytes(), synchronize);

                    channel.Remove(offset + 40);
                    channel.Receive(PeerCryptoPayload.MinimumSize, cryptoMessagePeek =>
                    {
                        RC4 decryptor = initiatorKey.Clone();
                        PeerMessage decrypted = cryptoMessagePeek.Decrypt(decryptor);
                        int size = PeerCryptoPayload.GetSize(decrypted);

                        channel.Receive(size, cryptoMessage =>
                        {
                            decryptor = initiatorKey.Clone();
                            decrypted = cryptoMessage.Decrypt(decryptor);
                            PeerCryptoPayload cryptoPayload = new PeerCryptoPayload(decrypted);

                            initiatorKey.Skip(size);
                            channel.Remove(size);

                            channel.Receive(PeerCryptoHandshake.MinimumSize, cryptoHandshakeMessagePeek =>
                            {
                                decryptor = initiatorKey.Clone();
                                decrypted = cryptoHandshakeMessagePeek.Decrypt(decryptor);
                                size = PeerCryptoHandshake.GetSize(decrypted);

                                channel.Receive(size, cryptoHandshakeMessage =>
                                {
                                    initiatorKey.Skip(size);
                                    channel.Remove(size);
                                    channel.Send(new PeerCryptoPayload(), receiverKey);

                                    channel.Receive(PeerHandshake.MinSize, handshakeMessagePeek =>
                                    {
                                        decryptor = initiatorKey.Clone();
                                        decrypted = handshakeMessagePeek.Decrypt(decryptor);
                                        size = PeerHandshake.GetSize(decrypted);

                                        channel.Receive(size, handshakeMessage =>
                                        {
                                            decryptor = initiatorKey.Clone();
                                            decrypted = handshakeMessage.Decrypt(decryptor);
                                            PeerHandshake received = new PeerHandshake(decrypted);

                                            initiatorKey.Skip(size);
                                            channel.Remove(size);

                                            channel.Send(handshake, receiverKey);
                                            channel.Continue(received, x => x.Encrypt(receiverKey), x => x.Decrypt(initiatorKey.Clone()), (buffer, count) => { buffer.Remove(count); initiatorKey.Skip(count); });
                                        });
                                    });
                                });
                            });
                        });
                    });
                });
            });
        }

        private static int Find(byte[] data, byte[] pattern)
        {
            for (int i = 0; i < data.Length - pattern.Length; i++)
            {
                bool success = true;

                for (int j = 0; j < pattern.Length; j++)
                {
                    if (data[j + i] != pattern[j])
                    {
                        success = false;
                        break;
                    }
                }

                if (success)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}