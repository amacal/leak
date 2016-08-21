using Leak.Core.Congestion;
using Leak.Core.Loop;
using Leak.Core.Messages;
using System;

namespace Leak.Core.Collector.Callbacks
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
                context.Cando.Start(channel.Endpoint.Peer);
            }

            context.Callback.OnHandshake(channel.Endpoint);
        }

        public override void OnKeepAlive(ConnectionLoopChannel channel)
        {
            context.Responder.Handle(channel.Endpoint.Peer, new KeepAliveMessage());
            context.Callback.OnIncoming(channel.Endpoint, new PeerCollectorMessage("keep-alive", 0));
        }

        public override void OnChoke(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            context.Congestion.SetChoking(channel.Endpoint.Peer, PeerCongestionDirection.Remote, true);
            context.Callback.OnIncoming(channel.Endpoint, message.ToConnector("choke"));
            context.Callback.OnChoke(channel.Endpoint, new ChokeMessage());
        }

        public override void OnUnchoke(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            context.Congestion.SetChoking(channel.Endpoint.Peer, PeerCongestionDirection.Remote, false);
            context.Callback.OnIncoming(channel.Endpoint, message.ToConnector("unchoke"));
            context.Callback.OnUnchoke(channel.Endpoint, new UnchokeMessage());
        }

        public override void OnInterested(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            context.Congestion.SetInterested(channel.Endpoint.Peer, PeerCongestionDirection.Remote, true);
            context.Callback.OnIncoming(channel.Endpoint, message.ToConnector("interested"));
            context.Callback.OnInterested(channel.Endpoint, new InterestedMessage());
        }

        public override void OnHave(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            context.Callback.OnIncoming(channel.Endpoint, message.ToConnector("have"));
            context.Callback.OnHave(channel.Endpoint, new HaveMessage());
        }

        public override void OnBitfield(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            BitfieldMessage payload = new BitfieldMessage(message.ToBytes());
            Bitfield bitfield = payload.ToBitfield();

            context.Battlefield.Handle(channel.Endpoint.Peer, bitfield);
            context.Callback.OnIncoming(channel.Endpoint, message.ToConnector("bitfield"));
            context.Callback.OnBitfield(channel.Endpoint, payload);
        }

        public override void OnPiece(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            context.Callback.OnIncoming(channel.Endpoint, message.ToConnector("piece"));
            context.Callback.OnPiece(channel.Endpoint, new PieceMessage(message.ToBytes()));
        }

        public override void OnExtended(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
            context.Cando.Handle(channel.Endpoint.Peer, new ExtendedIncomingMessage(message.ToBytes()));
            context.Callback.OnIncoming(channel.Endpoint, message.ToConnector("extended"));
        }

        public override void OnException(ConnectionLoopChannel channel, Exception ex)
        {
            lock (context.Synchronized)
            {
                context.Peers.Dismiss(channel.Endpoint.Peer);
                context.Battlefield.Remove(channel.Endpoint.Peer);

                context.Responder.Remove(channel.Endpoint.Peer);
                context.Storage.RemoveRemote(channel.Endpoint.Remote);
                context.Cando.Remove(channel.Endpoint.Peer);
            }
        }

        public override void OnDisconnected(ConnectionLoopChannel channel)
        {
            lock (context.Synchronized)
            {
                context.Peers.Dismiss(channel.Endpoint.Peer);
                context.Battlefield.Remove(channel.Endpoint.Peer);

                context.Responder.Remove(channel.Endpoint.Peer);
                context.Storage.RemoveRemote(channel.Endpoint.Remote);
                context.Cando.Remove(channel.Endpoint.Peer);
            }
        }
    }
}