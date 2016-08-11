using Leak.Core.Congestion;
using Leak.Core.Loop;
using Leak.Core.Messages;
using System;

namespace Leak.Core.Collector
{
    public class PeerCollectorLoop : ConnectionLoopCallbackBase
    {
        private readonly PeerCollectorContext context;

        public PeerCollectorLoop(PeerCollectorContext context)
        {
            this.context = context;
        }

        public override void OnAttached(ConnectionLoopChannel channel)
        {
            lock (context.Synchronized)
            {
                context.Communicator.Add(channel);
                context.Responder.Register(channel);
            }

            context.Callback.OnHandshake(channel.Endpoint);
        }

        public override void OnKeepAlive(ConnectionLoopChannel channel)
        {
            context.Responder.Handle(channel.Endpoint.Peer, new KeepAliveMessage());
            context.Callback.OnIncoming(channel.Endpoint.Peer, new PeerCollectorMessage("keep-alive", 0));
        }

        public override void OnChoke(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            context.Congestion.SetChoking(channel.Endpoint.Peer, PeerCongestionDirection.Remote, true);
            context.Callback.OnIncoming(channel.Endpoint.Peer, message.ToConnector("choke"));
            context.Callback.OnChoke(channel.Endpoint.Peer, new ChokeMessage());
        }

        public override void OnUnchoke(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            context.Congestion.SetChoking(channel.Endpoint.Peer, PeerCongestionDirection.Remote, false);
            context.Callback.OnIncoming(channel.Endpoint.Peer, message.ToConnector("unchoke"));
            context.Callback.OnUnchoke(channel.Endpoint.Peer, new UnchokeMessage());
        }

        public override void OnInterested(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            context.Congestion.SetInterested(channel.Endpoint.Peer, PeerCongestionDirection.Remote, true);
            context.Callback.OnIncoming(channel.Endpoint.Peer, message.ToConnector("interested"));
            context.Callback.OnInterested(channel.Endpoint.Peer, new InterestedMessage());
        }

        public override void OnHave(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            context.Callback.OnIncoming(channel.Endpoint.Peer, message.ToConnector("have"));
            context.Callback.OnHave(channel.Endpoint.Peer, new HaveMessage());
        }

        public override void OnBitfield(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            context.Callback.OnIncoming(channel.Endpoint.Peer, message.ToConnector("bitfield"));
            context.Callback.OnBitfield(channel.Endpoint.Peer, new BitfieldMessage(message.ToBytes()));
        }

        public override void OnPiece(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            context.Callback.OnIncoming(channel.Endpoint.Peer, message.ToConnector("piece"));
            context.Callback.OnPiece(channel.Endpoint.Peer, new PieceMessage(message.ToBytes()));
        }

        public override void OnExtended(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            context.Callback.OnIncoming(channel.Endpoint.Peer, message.ToConnector("extended"));
            context.Callback.OnExtended(channel.Endpoint.Peer, new ExtendedIncomingMessage(message.ToBytes()));
        }

        public override void OnException(ConnectionLoopChannel channel, Exception ex)
        {
            lock (context.Synchronized)
            {
                context.Peers.Dismiss(channel.Endpoint.Peer);
                context.Storage.RemoveRemote(channel.Endpoint.Remote);
            }
        }

        public override void OnDisconnected(ConnectionLoopChannel channel)
        {
            lock (context.Synchronized)
            {
                context.Peers.Dismiss(channel.Endpoint.Peer);
                context.Storage.RemoveRemote(channel.Endpoint.Remote);
            }
        }
    }
}