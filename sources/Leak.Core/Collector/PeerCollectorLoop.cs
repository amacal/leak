using Leak.Core.Loop;
using Leak.Core.Messages;
using System;

namespace Leak.Core.Collector
{
    public class PeerCollectorLoop : ConnectionLoopCallbackBase
    {
        private readonly PeerCollectorCallback callback;
        private readonly PeerCollectorStorage storage;
        private readonly PeerCollectorConfiguration configuration;
        private readonly object synchronized;

        public PeerCollectorLoop(PeerCollectorStorage storage, PeerCollectorConfiguration configuration, object synchronized)
        {
            this.storage = storage;
            this.callback = configuration.Callback;
            this.configuration = configuration;
            this.synchronized = synchronized;
        }

        public override void OnAttached(ConnectionLoopChannel channel)
        {
            lock (synchronized)
            {
                storage.AttachChannel(channel);
                callback.OnHandshake(channel.Endpoint);
            }
        }

        public override void OnKeepAlive(ConnectionLoopChannel channel)
        {
            callback.OnIncoming(channel.Endpoint.Peer, new PeerCollectorMessage("keep-alive", 0));
            callback.OnKeepAlive(channel.Endpoint.Peer, new KeepAliveMessage());
        }

        public override void OnChoke(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            lock (synchronized)
            {
                storage.SetChoked(channel.Endpoint.Peer, true);
                callback.OnIncoming(channel.Endpoint.Peer, message.ToConnector("choke"));
                callback.OnChoke(channel.Endpoint.Peer, new ChokeMessage());
            }
        }

        public override void OnUnchoke(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            lock (synchronized)
            {
                storage.SetChoked(channel.Endpoint.Peer, false);
                callback.OnIncoming(channel.Endpoint.Peer, message.ToConnector("unchoke"));
                callback.OnUnchoke(channel.Endpoint.Peer, new UnchokeMessage());
            }
        }

        public override void OnInterested(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            callback.OnIncoming(channel.Endpoint.Peer, message.ToConnector("interested"));
            callback.OnInterested(channel.Endpoint.Peer, new InterestedMessage());
        }

        public override void OnHave(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            callback.OnIncoming(channel.Endpoint.Peer, message.ToConnector("have"));
            callback.OnHave(channel.Endpoint.Peer, new HaveMessage());
        }

        public override void OnBitfield(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            configuration.Callback.OnIncoming(channel.Endpoint.Peer, message.ToConnector("bitfield"));
            configuration.Callback.OnBitfield(channel.Endpoint.Peer, new BitfieldMessage(message.ToBytes()));
        }

        public override void OnPiece(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            callback.OnIncoming(channel.Endpoint.Peer, message.ToConnector("piece"));
            callback.OnPiece(channel.Endpoint.Peer, new PieceMessage(message.ToBytes()));
        }

        public override void OnExtended(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            callback.OnIncoming(channel.Endpoint.Peer, message.ToConnector("extended"));
            callback.OnExtended(channel.Endpoint.Peer, new ExtendedIncomingMessage(message.ToBytes()));
        }

        public override void OnException(ConnectionLoopChannel channel, Exception ex)
        {
            lock (synchronized)
            {
                storage.RemoveRemote(channel.Endpoint.Remote);
            }
        }

        public override void OnDisconnected(ConnectionLoopChannel channel)
        {
            lock (synchronized)
            {
                storage.RemoveRemote(channel.Endpoint.Remote);
            }
        }
    }
}