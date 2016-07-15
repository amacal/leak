using Leak.Core.Net;
using Leak.Core.Network;

namespace Leak.Core.Negotiator
{
    public class HandshakeNegotiatorActive
    {
        private readonly HandshakeNegotiatorActiveContext context;
        private readonly HandshakeConnection connection;
        private readonly PeerCredentials credentials;
        private readonly HandshakeKeyContainer keys;

        public HandshakeNegotiatorActive(NetworkConnection connection, HandshakeNegotiatorActiveContext context)
        {
            this.context = context;
            this.connection = new HandshakeConnection(connection, context);

            this.credentials = PeerCryptography.Generate();
            this.keys = new HandshakeKeyContainer();
        }

        public void Execute()
        {
            connection.Send(new HandshakeKeyExchangeMessage(credentials));
            connection.Receive(HandleKeyExchangeMessage, 96);
        }

        private void HandleKeyExchangeMessage(NetworkIncomingMessage message)
        {
            HandshakeKeyExchange exchange = new HandshakeKeyExchange(message);

            keys.Secret = PeerCryptography.Secret(credentials, exchange.Key);
            keys.Local = new HandshakeKey(HandshakeKeyOwnership.Initiator, keys.Secret, context.Hash);
            keys.Remote = new HandshakeKey(HandshakeKeyOwnership.Receiver, keys.Secret, context.Hash);

            message.Acknowledge(96);

            connection.Send(new HandshakeCryptoHashMessage(keys.Secret, context.Hash.ToBytes()));
            connection.Send(new HandshakeCryptoPayloadMessage(), keys.Local);
            connection.Send(new HandshakeCryptoMessage(), keys.Local);

            connection.Receive(SynchronizeCryptoMessage, VerifyCryptoMessage);
        }

        private bool VerifyCryptoMessage(NetworkIncomingMessage message)
        {
            HandshakeKey decryptor = keys.Remote.Clone();
            byte[] synchronize = HandshakeCryptoPayload.GetVerification();

            byte[] encrypted = decryptor.Decrypt(synchronize);
            int offset = Bytes.Find(message.ToBytes(), encrypted);

            return offset >= 0 && message.Length >= offset + HandshakeCryptoPayload.MinimumSize;
        }

        private void SynchronizeCryptoMessage(NetworkIncomingMessage message)
        {
            HandshakeKey decryptor = keys.Remote.Clone();
            byte[] synchronize = HandshakeCryptoPayload.GetVerification();

            byte[] encrypted = decryptor.Encrypt(synchronize);
            int offset = Bytes.Find(message.ToBytes(), encrypted);

            message.Acknowledge(offset);
            connection.Receive(MeasureCryptoMessage, HandshakeCryptoPayload.MinimumSize);
        }

        private void MeasureCryptoMessage(NetworkIncomingMessage message)
        {
            HandshakeKey decryptor = keys.Remote.Clone();
            NetworkIncomingMessage decrypted = decryptor.Decrypt(message);

            int size = HandshakeCryptoPayload.GetSize(decrypted);
            connection.Receive(HandleCryptoMessage, size);
        }

        private void HandleCryptoMessage(NetworkIncomingMessage message)
        {
            HandshakeKey decryptor = keys.Remote.Clone();
            NetworkIncomingMessage decrypted = decryptor.Decrypt(message);
            int size = HandshakeCryptoPayload.GetSize(decrypted);

            message.Acknowledge(size);
            keys.Remote.Acknowledge(size);

            connection.Send(new HandshakeMessage(context.Peer, context.Hash, context.Options), keys.Local);
            connection.Receive(MeasureHandshakeMessage, HandshakeMessage.MinSize);
        }

        private void MeasureHandshakeMessage(NetworkIncomingMessage message)
        {
            HandshakeKey decryptor = keys.Remote.Clone();
            NetworkIncomingMessage decrypted = decryptor.Decrypt(message);

            int size = HandshakeMessage.GetSize(decrypted);
            connection.Receive(HandleHandshakeMessage, size);
        }

        private void HandleHandshakeMessage(NetworkIncomingMessage message)
        {
            HandshakeKey decryptor = keys.Remote.Clone();
            NetworkIncomingMessage decrypted = decryptor.Decrypt(message);

            int size = HandshakeMessage.GetSize(decrypted);
            Handshake handshake = new Handshake(context.Peer, context.Peer, context.Hash);

            message.Acknowledge(size);
            keys.Remote.Acknowledge(size);

            context.OnHandshake(connection.StartEncryption(keys), handshake);
        }
    }
}