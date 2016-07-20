using Leak.Core.Loop;
using Leak.Core.Messages;
using System;

namespace Leak.Core.Collector
{
    public class PeerCollectorToLoop : ConnectionLoopCallbackBase
    {
        private readonly PeerCollectorStorage storage;
        private readonly PeerCollectorConfiguration configuration;

        public PeerCollectorToLoop(PeerCollectorStorage storage, PeerCollectorConfiguration configuration)
        {
            this.storage = storage;
            this.configuration = configuration;
        }

        public override void OnConnected(ConnectionLoopChannel channel)
        {
            storage.Add(channel);
        }

        public override void OnKeepAlive(ConnectionLoopChannel channel)
        {
            configuration.Callback.OnKeepAlive(channel.Peer, new KeepAliveMessage());
        }

        public override void OnUnchoke(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            configuration.Callback.OnUnchoke(channel.Peer, new UnchokeMessage());
        }

        public override void OnInterested(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            configuration.Callback.OnInterested(channel.Peer, new InterestedMessage());
        }

        public override void OnBitfield(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            configuration.Callback.OnBitfield(channel.Peer, new BitfieldMessage(message.ToBytes()));
        }

        public override void OnPiece(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            configuration.Callback.OnPiece(channel.Peer, new PieceMessage(message.ToBytes()));
        }

        public override void OnException(ConnectionLoopChannel channel, Exception ex)
        {
            storage.Remove(channel.Peer);
        }

        public override void OnDisconnected(ConnectionLoopChannel channel)
        {
            storage.Remove(channel.Peer);
        }
    }
}