using System.Collections.Generic;
using Leak.Common;
using Leak.Extensions.Metadata;
using Leak.Meta.Get;
using Leak.Peer.Coordinator;

namespace Leak.Client.Adapters
{
    internal class MetaGetToGlueAdapter : MetagetGlue
    {
        private readonly CoordinatorService service;

        public MetaGetToGlueAdapter(CoordinatorService service)
        {
            this.service = service;
        }

        public IEnumerable<PeerHash> Peers
        {
            get
            {
                List<PeerHash> peers = new List<PeerHash>();

                service.ForEachPeer(peer =>
                {
                    if (service.IsSupported(peer, MetadataPlugin.Name))
                    {
                        peers.Add(peer);
                    }
                });

                return peers;
            }
        }

        public void SendMetadataRequest(PeerHash peer, int piece)
        {
            service.SendMetadataRequest(peer, piece);
        }
    }
}