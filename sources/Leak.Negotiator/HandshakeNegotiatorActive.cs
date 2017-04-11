using Leak.Common;
using Leak.Networking;
using Leak.Networking.Core;

namespace Leak.Peer.Negotiator
{
    public class HandshakeNegotiatorActive
    {
        private readonly HandshakeNegotiatorActiveContext context;
        private readonly HandshakeNegotiatorHooks hooks;
        private readonly HandshakeConnection connection;
        private readonly HandshakeCredentials credentials;
        private readonly HandshakeKeyContainer keys;

        public HandshakeNegotiatorActive(NetworkPool pool, NetworkConnection connection, HandshakeNegotiatorActiveContext context, HandshakeNegotiatorHooks hooks)
        {
            this.context = context;
            this.hooks = hooks;

            this.connection = new HandshakeConnection(pool, connection, hooks);

            this.credentials = HandshakeCryptography.Generate();
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

            keys.Secret = HandshakeCryptography.Secret(credentials, exchange.Key);
            keys.Local = new HandshakeKey(HandshakeKeyOwnership.Initiator, keys.Secret, context.Hash);
            keys.Remote = new HandshakeKey(HandshakeKeyOwnership.Receiver, keys.Secret, context.Hash);

            message.Acknowledge(96);

            connection.Send(new HandshakeCryptoHashMessage(keys.Secret, context.Hash.ToBytes()));
            connection.Send(new HandshakeCryptoPayloadMessage(2), keys.Local);
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

            byte[] encrypted = decryptor.Encrypt(synchronize, 0, synchronize.Length);
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
            PeerHash peer = HandshakeMessage.GetPeer(decrypted);

            HandshakeOptions options = HandshakeMessage.GetOptions(decrypted);
            Handshake handshake = new Handshake(context.Peer, peer, context.Hash, options);

            message.Acknowledge(size);
            keys.Remote.Acknowledge(size);

            NetworkConnection other = connection.StartEncryption(keys);

            hooks.CallHandshakeCompleted(other, handshake);
            context.OnHandshake(other, handshake);
        }
    }
}