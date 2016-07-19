using Leak.Core.Collector;
using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Metadata;
using Leak.Core.Repository;
using Leak.Core.Retriever;
using System.Collections.Generic;
using System.IO;

namespace Leak.Core.Client
{
    public class PeerClientStorage
    {
        private readonly PeerClientConfiguration configuration;
        private readonly PeerClientStorageEntryCollection collection;

        public PeerClientStorage(PeerClientConfiguration configuration)
        {
            this.configuration = configuration;
            this.collection = new PeerClientStorageEntryCollection();
        }

        public void Register(MetainfoFile metainfo, PeerCollectorView collector)
        {
            string root = configuration.Destination;
            string hash = metainfo.Data.Hash.ToString();
            string path = Path.Combine(root, hash);

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            ResourceRepository repository = new ResourceRepository(metainfo.Data, path);
            ResourceRetriever retriever = new ResourceRetriever(repository, collector);

            collection.Add(metainfo.Data.Hash, new PeerClientStorageEntry
            {
                Metainfo = metainfo.Data,
                Repository = repository,
                Retriever = retriever,
                Peers = new HashSet<PeerHash>()
            });
        }

        public void AddPeer(FileHash hash, PeerHash peer)
        {
            collection.AddPeer(hash, peer);
            collection.ByHash(hash).Peers.Add(peer);
            configuration.Callback.OnPeerConnected(collection.ByHash(hash).Metainfo, peer);
        }

        public void AddBitfield(PeerHash peer, Bitfield bitfield)
        {
            configuration.Callback.OnPeerBitfield(collection.ByPeer(peer).Metainfo, peer, bitfield);
        }

        public ResourceRepository GetRepository(FileHash hash)
        {
            return collection.ByHash(hash).Repository;
        }

        public ResourceRetriever GetRetriever(FileHash hash)
        {
            return collection.ByHash(hash).Retriever;
        }
    }
}