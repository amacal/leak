using Leak.Core.Network;
using System;

namespace Leak.Core.Net
{
    public class PeerNegotiatorEncrypted : PeerNegotiator
    {
        private readonly PeerNegotiatorEncryptedConfiguration configuration;

        public PeerNegotiatorEncrypted(Action<PeerNegotiatorEncryptedConfiguration> configurator)
        {
            configuration = new PeerNegotiatorEncryptedConfiguration
            {
                Credentials = PeerCryptography.Generate()
            };

            configurator.Invoke(configuration);
        }

        public void Active(PeerNegotiatorActiveContext context)
        {
            context.Connection.Send(new PeerKeyExchange(configuration.Credentials));
            context.Connection.Receive(96, keyExchangeMessage =>
            {
                PeerKeyExchange exchange = new PeerKeyExchange(keyExchangeMessage);
                byte[] secret = PeerCryptography.Secret(configuration.Credentials, exchange.Key);

                RC4 initiatorKey = new RC4(Bytes.Hash("keyA", secret, context.Hash), 1024);
                RC4 receiverKey = new RC4(Bytes.Hash("keyB", secret, context.Hash), 1024);

                keyExchangeMessage.Acknowledge(96);
                context.Connection.Send(new PeerCryptoHash(secret, context.Hash));
                context.Connection.Send(new PeerCryptoPayload(), initiatorKey);
                context.Connection.Send(new PeerCryptoHandshake(), initiatorKey);

                Func<NetworkIncomingMessage, bool> hasCrypto = message =>
                {
                    RC4 decryptor = receiverKey.Clone();
                    byte[] synchronize = PeerCryptoPayload.GetVerification();

                    byte[] encrypted = synchronize.Encrypt(decryptor);
                    int offset = Find(message.ToBytes(), encrypted);

                    return offset >= 0 && message.Length >= offset + PeerCryptoPayload.MinimumSize;
                };

                context.Connection.Receive(hasCrypto, cryptoMessageSync =>
                {
                    RC4 decryptor = receiverKey.Clone();
                    byte[] synchronize = PeerCryptoPayload.GetVerification();

                    byte[] encrypted = synchronize.Encrypt(decryptor);
                    int offset = Find(cryptoMessageSync.ToBytes(), encrypted);

                    cryptoMessageSync.Acknowledge(offset);
                    context.Connection.Receive(PeerCryptoPayload.MinimumSize, cryptoMessagePeek =>
                    {
                        decryptor = receiverKey.Clone();
                        NetworkIncomingMessage decrypted = cryptoMessagePeek.Decrypt(decryptor);
                        int size = PeerCryptoPayload.GetSize(decrypted);

                        context.Connection.Receive(size, cryptoMessage =>
                        {
                            receiverKey.Skip(size);
                            cryptoMessage.Acknowledge(size);
                            context.Connection.Send(new PeerHandshakePayload(context.Hash, context.Hash, context.Options), initiatorKey);

                            context.Connection.Receive(PeerHandshakePayload.MinSize, handshakeMessagePeek =>
                            {
                                decryptor = receiverKey.Clone();
                                decrypted = handshakeMessagePeek.Decrypt(decryptor);
                                size = PeerHandshakePayload.GetSize(decrypted);

                                context.Connection.Receive(size, handshakeMessage =>
                                {
                                    decryptor = receiverKey.Clone();
                                    decrypted = handshakeMessage.Decrypt(decryptor);
                                    PeerHandshakePayload received = new PeerHandshakePayload(decrypted);

                                    receiverKey.Skip(size);
                                    handshakeMessage.Acknowledge(size);

                                    context.Continue(received, context.Connection.StartEncryption(initiatorKey, receiverKey));
                                });
                            });
                        });
                    });
                });
            });
        }

        public void Passive(PeerNegotiatorPassiveContext context)
        {
            context.Connection.Receive(96, keyExchangeMessage =>
            {
                PeerKeyExchange exchange = new PeerKeyExchange(keyExchangeMessage);
                byte[] secret = PeerCryptography.Secret(configuration.Credentials, exchange.Key);

                keyExchangeMessage.Acknowledge(96);
                context.Connection.Send(new PeerKeyExchange(configuration.Credentials));

                Func<NetworkIncomingMessage, bool> hasHashes = message =>
                {
                    byte[] synchronize = PeerCryptoHash.GetHash(secret);
                    int offset = Find(message.ToBytes(), synchronize);

                    return offset >= 0 && message.Length >= offset + 40;
                };

                context.Connection.Receive(hasHashes, hashMessage =>
                {
                    byte[] synchronize = PeerCryptoHash.GetHash(secret);
                    int offset = Find(hashMessage.ToBytes(), synchronize);

                    byte[] hash = hashMessage.ToBytes(offset + 20, 20);
                    byte[] matched = context.Hashes.Find(secret, hash);

                    if (matched == null)
                    {
                        context.Terminate();
                        return;
                    }

                    RC4 initiatorKey = new RC4(Bytes.Hash("keyA", secret, matched), 1024);
                    RC4 receiverKey = new RC4(Bytes.Hash("keyB", secret, matched), 1024);

                    hashMessage.Acknowledge(offset + 40);
                    context.Connection.Receive(PeerCryptoPayload.MinimumSize, cryptoMessagePeek =>
                    {
                        RC4 decryptor = initiatorKey.Clone();
                        NetworkIncomingMessage decrypted = cryptoMessagePeek.Decrypt(decryptor);
                        int size = PeerCryptoPayload.GetSize(decrypted);

                        context.Connection.Receive(size, cryptoMessage =>
                        {
                            decryptor = initiatorKey.Clone();
                            decrypted = cryptoMessage.Decrypt(decryptor);
                            PeerCryptoPayload cryptoPayload = new PeerCryptoPayload(decrypted);

                            initiatorKey.Skip(size);
                            cryptoMessage.Acknowledge(size);

                            context.Connection.Receive(PeerCryptoHandshake.MinimumSize, cryptoHandshakeMessagePeek =>
                            {
                                decryptor = initiatorKey.Clone();
                                decrypted = cryptoHandshakeMessagePeek.Decrypt(decryptor);
                                size = PeerCryptoHandshake.GetSize(decrypted);

                                context.Connection.Receive(size, cryptoHandshakeMessage =>
                                {
                                    initiatorKey.Skip(size);
                                    cryptoHandshakeMessage.Acknowledge(size);
                                    context.Connection.Send(new PeerCryptoPayload(), receiverKey);

                                    context.Connection.Receive(PeerHandshakePayload.MinSize, handshakeMessagePeek =>
                                    {
                                        decryptor = initiatorKey.Clone();
                                        decrypted = handshakeMessagePeek.Decrypt(decryptor);
                                        size = PeerHandshakePayload.GetSize(decrypted);

                                        context.Connection.Receive(size, handshakeMessage =>
                                        {
                                            decryptor = initiatorKey.Clone();
                                            decrypted = handshakeMessage.Decrypt(decryptor);
                                            PeerHandshakePayload received = new PeerHandshakePayload(decrypted);

                                            initiatorKey.Skip(size);
                                            handshakeMessage.Acknowledge(size);

                                            context.Connection.Send(new PeerHandshakePayload(matched, matched, context.Options), receiverKey);
                                            context.Continue(received, context.Connection.StartEncryption(receiverKey, initiatorKey));
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