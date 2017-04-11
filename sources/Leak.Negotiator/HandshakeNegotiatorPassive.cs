using Leak.Common;
using Leak.Networking;
using Leak.Networking.Core;

namespace Leak.Peer.Negotiator
{
    public class HandshakeNegotiatorPassive
    {
        private readonly HandshakeNegotiatorPassiveContext context;
        private readonly HandshakeNegotiatorHooks hooks;
        private readonly HandshakeConnection connection;
        private readonly HandshakeCredentials credentials;
        private readonly HandshakeKeyContainer keys;

        private FileHash found;

        public HandshakeNegotiatorPassive(NetworkPool pool, NetworkConnection connection, HandshakeNegotiatorPassiveContext context, HandshakeNegotiatorHooks hooks)
        {
            this.context = context;
            this.hooks = hooks;

            this.connection = new HandshakeConnection(pool, connection, hooks);
            this.credentials = HandshakeCryptography.Generate();
            this.keys = new HandshakeKeyContainer();
        }

        public void Execute()
        {
            connection.Receive(HandleKeyExchangeMessage, 96);
        }

        private void HandleKeyExchangeMessage(NetworkIncomingMessage message)
        {
            HandshakeKeyExchange exchange = new HandshakeKeyExchange(message);

            keys.Secret = HandshakeCryptography.Secret(credentials, exchange.Key);
            message.Acknowledge(96);

            connection.Send(new HandshakeKeyExchangeMessage(credentials));
            connection.Receive(SynchronizeCryptoHashMessage, VerifyCryptoHashMessage);
        }

        private bool VerifyCryptoHashMessage(NetworkIncomingMessage message)
        {
            byte[] synchronize = HandshakeCryptoHashMessage.GetHash(keys.Secret);
            int offset = Bytes.Find(message.ToBytes(), synchronize);

            return offset >= 0 && message.Length >= offset + 40;
        }

        private void SynchronizeCryptoHashMessage(NetworkIncomingMessage message)
        {
            byte[] synchronize = HandshakeCryptoHashMessage.GetHash(keys.Secret);
            int offset = Bytes.Find(message.ToBytes(), synchronize);

            byte[] bytes = message.ToBytes(offset + 20, 20);
            HandshakeMatch match = new HandshakeMatch(keys.Secret, bytes);

            found = context.Hashes.Find(match);

            if (found == null)
            {
                context.OnRejected(new HandshakeRejection(match));
                connection.CallHandshakeRejected();
                connection.Terminate();

                return;
            }

            keys.Local = new HandshakeKey(HandshakeKeyOwnership.Receiver, keys.Secret, found);
            keys.Remote = new HandshakeKey(HandshakeKeyOwnership.Initiator, keys.Secret, found);

            message.Acknowledge(offset + 40);
            connection.Receive(MeasureCryptoPayloadMessage, HandshakeCryptoPayload.MinimumSize);
        }

        private void MeasureCryptoPayloadMessage(NetworkIncomingMessage message)
        {
            HandshakeKey decryptor = keys.Remote.Clone();
            NetworkIncomingMessage decrypted = decryptor.Decrypt(message);

            int size = HandshakeCryptoPayload.GetSize(decrypted);
            connection.Receive(HandleCryptoPayloadMessage, size);
        }

        private void HandleCryptoPayloadMessage(NetworkIncomingMessage message)
        {
            HandshakeKey decryptor = keys.Remote.Clone();
            NetworkIncomingMessage decrypted = decryptor.Decrypt(message);

            int size = HandshakeCryptoPayload.GetSize(decrypted);
            int method = HandshakeCryptoPayload.GetMethod(decrypted);

            message.Acknowledge(size);
            keys.Remote.Acknowledge(size);

            connection.Receive(MeasureCryptoMessage, HandshakeCryptoMessage.MinimumSize, method);
        }

        private void MeasureCryptoMessage(NetworkIncomingMessage message, int method)
        {
            HandshakeKey decryptor = keys.Remote.Clone();
            NetworkIncomingMessage decrypted = decryptor.Decrypt(message);

            int size = HandshakeCryptoMessage.GetSize(decrypted);
            connection.Receive(HandleCryptoMessage, size, method);
        }

        private void HandleCryptoMessage(NetworkIncomingMessage message, int method)
        {
            message.Acknowledge(2);
            keys.Remote.Acknowledge(2);

            connection.Send(new HandshakeCryptoPayloadMessage(2), keys.Local);
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
            PeerHash peer = HandshakeMessage.GetPeer(decrypted);

            HandshakeOptions options = HandshakeMessage.GetOptions(decrypted);
            Handshake handshake = new Handshake(context.Peer, peer, found, options);

            message.Acknowledge(size);
            keys.Remote.Acknowledge(size);

            NetworkConnection other = connection.StartEncryption(keys);

            connection.Send(new HandshakeMessage(context.Peer, found, context.Options), keys.Local);
            context.OnHandshake(other, handshake);
            hooks.CallHandshakeCompleted(other, handshake);
        }
    }
}