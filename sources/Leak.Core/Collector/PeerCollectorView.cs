using Leak.Core.Cando.Metadata;
using Leak.Core.Common;
using Leak.Core.Communicator;
using Leak.Core.Congestion;
using Leak.Core.Messages;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Collector
{
    public class PeerCollectorView
    {
        private readonly PeerCollectorContext context;
        private readonly FileHash hash;

        public PeerCollectorView(FileHash hash, PeerCollectorContext context)
        {
            this.hash = hash;
            this.context = context;
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public PeerHash[] GetPeers(params PeerCollectorCriterion[] criterions)
        {
            IEnumerable<PeerHash> peers;

            lock (context.Synchronized)
            {
                peers = context.Peers.Find(hash);

                foreach (PeerCollectorCriterion criterion in criterions)
                {
                    peers = criterion.Accept(peers, context);
                }
            }

            return peers.ToArray();
        }

        public Bitfield GetBitfield(PeerHash peer)
        {
            return context.Battlefield.Get(peer);
        }

        public void Increase(PeerHash peer, int step)
        {
            context.Ranking.Increase(peer, step);
        }

        public void Decrease(PeerHash peer, int step)
        {
            context.Ranking.Decrease(peer, step);
        }

        public void SendLocalInterested(PeerHash peer)
        {
            lock (context.Synchronized)
            {
                InterestedMessage interested = new InterestedMessage();
                PeerCongestionDirection direction = PeerCongestionDirection.Local;

                context.Communicator.Get(peer)?.Send(interested);
                context.Congestion.SetInterested(peer, direction, true);
            }
        }

        public void SendBitfield(PeerHash peer, Bitfield bitfield)
        {
            lock (context.Synchronized)
            {
                BitfieldMessage message = new BitfieldMessage(bitfield.Length);
                CommunicatorChannel channel = context.Communicator.Get(peer);

                channel?.Send(message);
            }
        }

        public void SendPieceRequest(PeerHash peer, Request[] requests)
        {
            lock (context.Synchronized)
            {
                CommunicatorChannel channel = context.Communicator.Get(peer);
                List<RequestOutgoingMessage> messages = new List<RequestOutgoingMessage>();

                foreach (Request request in requests)
                {
                    messages.Add(new RequestOutgoingMessage(request));
                }

                channel.Send(messages.ToArray());
            }
        }

        public void SendMetadataRequest(PeerHash peer, int block)
        {
            lock (context.Synchronized)
            {
                context.Cando.Send(peer, formatter => formatter.MetadataRequest(block));
            }
        }
    }
}