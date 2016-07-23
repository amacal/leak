using Leak.Core.Collector;
using Leak.Core.Common;
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
        private readonly PeerClientCallback callback;

        public PeerClientStorage(PeerClientConfiguration configuration)
        {
            this.configuration = configuration;
            this.callback = configuration.Callback;

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

            ResourceRetriever retriever = new ResourceRetriever(with =>
            {
                with.Repository = repository;
                with.Collector = collector;
                with.Callback = new PeerClientToRetriever(metainfo.Data, configuration);
            });

            collection.Add(metainfo.Data.Hash, new PeerClientStorageEntry
            {
                Metainfo = metainfo.Data,
                Repository = repository,
                Retriever = retriever,
                Peers = new HashSet<PeerHash>()
            });
        }

        public bool Contains(string remote)
        {
            return collection.Contains(remote);
        }

        public bool AddEndpoint(PeerEndpoint endpoint)
        {
            return collection.AddEndpoint(endpoint);
        }

        public void RemovePeer(PeerHash peer)
        {
            collection.ByPeer(peer).Peers.Remove(peer);
        }

        public ResourceRepository GetRepository(FileHash hash)
        {
            return collection.ByHash(hash).Repository;
        }

        public ResourceRetriever GetRetriever(FileHash hash)
        {
            return collection.ByHash(hash).Retriever;
        }

        public ResourceRetriever GetRetriever(PeerHash peer)
        {
            return collection.ByPeer(peer).Retriever;
        }

        public Metainfo GetMetainfo(PeerHash peer)
        {
            return collection.ByPeer(peer).Metainfo;
        }

        public Metainfo GetMetainfo(FileHash hash)
        {
            return collection.ByHash(hash).Metainfo;
        }
    }
}