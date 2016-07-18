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
        private readonly Dictionary<FileHash, PeerClientStorageEntry> entries;

        public PeerClientStorage(PeerClientConfiguration configuration)
        {
            this.configuration = configuration;
            this.entries = new Dictionary<FileHash, PeerClientStorageEntry>();
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

            entries.Add(metainfo.Data.Hash, new PeerClientStorageEntry
            {
                Metainfo = metainfo.Data,
                Repository = repository,
                Retriever = retriever
            });
        }

        public ResourceRepository GetRepository(FileHash hash)
        {
            return entries[hash].Repository;
        }

        public ResourceRetriever GetRetriever(FileHash hash)
        {
            return entries[hash].Retriever;
        }
    }
}