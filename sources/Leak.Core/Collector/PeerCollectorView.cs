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

        public bool IsRemoteChoking(PeerHash peer)
        {
            lock (context.Synchronized)
            {
                return context.Congestion.IsChoking(peer, PeerCongestionDirection.Remote);
            }
        }

        public void SetLocalInterested(PeerHash peer)
        {
            lock (context.Synchronized)
            {
                InterestedMessage interested = new InterestedMessage();
                PeerCongestionDirection direction = PeerCongestionDirection.Local;

                context.Communicator.Get(peer)?.Send(interested);
                context.Congestion.IsInterested(peer, direction);
            }
        }

        public PeerHash[] GetPeers(params PeerCollectorCriterion[] criterions)
        {
            HashSet<PeerHash> peers = new HashSet<PeerHash>();

            lock (context.Synchronized)
            {
                foreach (PeerHash peer in context.Peers.Find(hash))
                {
                    peers.Add(peer);

                    foreach (PeerCollectorCriterion criterion in criterions)
                    {
                        if (criterion.Accept(peer, context) == false)
                        {
                            peers.Remove(peer);
                            break;
                        }
                    }
                }
            }

            return peers.ToArray();
        }

        public bool SupportExtensions(PeerHash peer)
        {
            lock (context.Synchronized)
            {
                return context.Storage.SupportsExtensions(peer);
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