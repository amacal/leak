using Leak.Core.Common;
using Leak.Core.Messages;
using System.Collections.Generic;

namespace Leak.Core.Retriever
{
    public class ResourceBitfield
    {
        private readonly ResourceConfiguration configuration;
        private readonly ResourceBitfieldMap completed;
        private readonly Dictionary<PeerHash, Bitfield> peers;
        private readonly HashSet<ResourcePieceRequest> frozen;

        public ResourceBitfield(ResourceConfiguration configuration)
        {
            this.configuration = configuration;
            this.completed = new ResourceBitfieldMap(configuration);
            this.peers = new Dictionary<PeerHash, Bitfield>();
            this.frozen = new HashSet<ResourcePieceRequest>();
        }

        public void Add(PeerHash peer, Bitfield bitfield)
        {
            peers.Add(peer, bitfield);
        }

        public int Count(int piece)
        {
            int count = 0;

            foreach (Bitfield bitfield in peers.Values)
            {
                if (bitfield[piece])
                {
                    count++;
                }
            }

            return count;
        }

        public bool Complete(int piece, int block)
        {
            return completed.Complete(piece, block);
        }

        public void Invalidate(int piece)
        {
            completed.Invalidate(piece);
        }

        public bool IsComplete(int piece)
        {
            return completed.IsComplete(piece);
        }

        public ResourcePieceRequest[] Next(PeerHash peer, int count)
        {
            Bitfield bitfield = peers[peer];
            List<ResourcePieceRequest> requests = new List<ResourcePieceRequest>();

            for (int i = 0; i < configuration.Pieces; i++)
            {
                if (bitfield[i] && completed.IsComplete(i) == false)
                {
                    for (int j = 0; j < configuration.BlocksInPiece; j++)
                    {
                        if (completed.IsComplete(i, j) == false)
                        {
                            int offset = j * configuration.BlockSize;
                            ResourcePieceRequest request = new ResourcePieceRequest(i, offset);

                            if (frozen.Contains(request) == false)
                            {
                                requests.Add(request);
                            }

                            if (requests.Count == count)
                            {
                                return requests.ToArray();
                            }
                        }
                    }
                }
            }

            return requests.ToArray();
        }

        public void Freeze(PeerHash peer, ResourcePieceRequest request)
        {
            frozen.Add(request);
        }
    }
}