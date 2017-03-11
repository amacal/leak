using System.Collections.Generic;
using Leak.Common;
using Leak.Extensions.Metadata;
using Leak.Glue;
using Leak.Metafile;
using Leak.Metaget;

namespace Leak.Client.Peer
{
    public static class PeerExtensions
    {
        public static MetagetGlue AsMetaGet(this GlueService service)
        {
            return new MetaGetGlueForwarder(service);
        }

        public static MetagetMetafile AsMetaGet(this MetafileService service)
        {
            return new MetaGetToMetaStoreForwarder(service);
        }

        private class MetaGetGlueForwarder : MetagetGlue
        {
            private readonly GlueService service;

            public MetaGetGlueForwarder(GlueService service)
            {
                this.service = service;
            }

            public IEnumerable<PeerHash> Peers
            {
                get
                {
                    List<PeerHash> peers = new List<PeerHash>();

                    service.ForEachPeer(peers.Add);
                    return peers;
                }
            }

            public void SendMetadataRequest(PeerHash peer, int piece)
            {
                service.SendMetadataRequest(peer, piece);
            }
        }

        private class MetaGetToMetaStoreForwarder : MetagetMetafile
        {
            private readonly MetafileService service;

            public MetaGetToMetaStoreForwarder(MetafileService service)
            {
                this.service = service;
            }

            public bool IsCompleted()
            {
                return service.IsCompleted();
            }

            public void Write(int piece, byte[] data)
            {
                service.Write(piece, data);
            }

            public void Verify()
            {
                service.Verify();
            }
        }
    }
}