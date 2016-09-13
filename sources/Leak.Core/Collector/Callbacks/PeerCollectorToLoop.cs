using Leak.Core.Congestion;
using Leak.Core.Loop;
using Leak.Core.Messages;
using System;

namespace Leak.Core.Collector.Callbacks
{
    public class PeerCollectorToLoop : ConnectionLoopCallbackBase
    {
        private readonly PeerCollectorContext context;

        public PeerCollectorToLoop(PeerCollectorContext context)
        {
            this.context = context;
        }

        public override void OnAttached(ConnectionLoopChannel channel)
        {
            lock (context.Synchronized)
            {
                context.Communicator.Add(channel);
                context.Responder.Register(channel);
                context.Cando.Start(channel.Endpoint.Session);
            }

            context.Callback.OnHandshake(channel.Endpoint);
        }

        public override void OnKeepAlive(ConnectionLoopChannel channel)
        {
            context.Responder.Handle(channel.Endpoint.Session.Peer, new KeepAliveMessage());
            context.Callback.OnIncoming(channel.Endpoint, new PeerCollectorMessage("keep-alive", 0));
        }

        public override void OnChoke(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            context.Congestion.SetChoking(channel.Endpoint.Session.Peer, PeerCongestionDirection.Remote, true);
            context.Callback.OnIncoming(channel.Endpoint, message.ToConnector("choke"));
            context.Callback.OnChoke(channel.Endpoint, new ChokeMessage());
        }

        public override void OnUnchoke(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            context.Congestion.SetChoking(channel.Endpoint.Session.Peer, PeerCongestionDirection.Remote, false);
            context.Callback.OnIncoming(channel.Endpoint, message.ToConnector("unchoke"));
            context.Callback.OnUnchoke(channel.Endpoint, new UnchokeMessage());
        }

        public override void OnInterested(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            context.Congestion.SetInterested(channel.Endpoint.Session.Peer, PeerCongestionDirection.Remote, true);
            context.Callback.OnIncoming(channel.Endpoint, message.ToConnector("interested"));
            context.Callback.OnInterested(channel.Endpoint, new InterestedMessage());
        }

        public override void OnHave(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            HaveMessage payload = new HaveMessage(message.ToBytes());

            context.Battlefield.Handle(channel.Endpoint.Session, payload);
            context.Callback.OnIncoming(channel.Endpoint, message.ToConnector("have"));
        }

        public override void OnBitfield(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            BitfieldMessage payload = new BitfieldMessage(message.ToBytes());

            context.Battlefield.Handle(channel.Endpoint.Session, payload);
            context.Callback.OnIncoming(channel.Endpoint, message.ToConnector("bitfield"));
        }

        public override void OnPiece(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            context.Callback.OnIncoming(channel.Endpoint, message.ToConnector("piece"));
            context.Callback.OnPiece(channel.Endpoint, new PieceMessage(message.ToBlock(context.BlockFactory)));
        }

        public override void OnExtended(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            context.Cando.Handle(channel.Endpoint.Session, new ExtendedIncomingMessage(message.ToBytes()));
            context.Callback.OnIncoming(channel.Endpoint, message.ToConnector("extended"));
        }

        public override void OnException(ConnectionLoopChannel channel, Exception ex)
        {
            lock (context.Synchronized)
            {
                context.Peers.Dismiss(channel.Endpoint.Session.Peer);
                context.Battlefield.Remove(channel.Endpoint.Session);

                context.Responder.Remove(channel.Endpoint.Session.Peer);
                context.Cando.Remove(channel.Endpoint.Session);
            }

            context.Callback.OnDisconnected(channel.Endpoint.Session);
        }

        public override void OnDisconnected(ConnectionLoopChannel channel)
        {
            lock (context.Synchronized)
            {
                context.Peers.Dismiss(channel.Endpoint.Session.Peer);
                context.Battlefield.Remove(channel.Endpoint.Session);

                context.Responder.Remove(channel.Endpoint.Session.Peer);
                context.Cando.Remove(channel.Endpoint.Session);
            }

            context.Callback.OnDisconnected(channel.Endpoint.Session);
        }
    }
}