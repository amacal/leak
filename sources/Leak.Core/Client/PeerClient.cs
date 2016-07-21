using Leak.Core.Collector;
using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Messages;
using Leak.Core.Metadata;
using Leak.Core.Repository;
using Leak.Core.Retriever;
using Leak.Core.Tracker;
using System;

namespace Leak.Core.Client
{
    public class PeerClient
    {
        private readonly PeerCollector collector;
        private readonly PeerClientStorage storage;
        private readonly PeerClientConfiguration configuration;

        public PeerClient(Action<PeerClientConfiguration> configurer)
        {
            this.configuration = new PeerClientConfiguration
            {
                Peer = PeerHash.Random(),
                Destination = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.Create),
                Callback = new PeerClientCallbackToNothing()
            };

            configurer.Invoke(configuration);

            this.storage = new PeerClientStorage(configuration);
            this.collector = new PeerCollector(with =>
            {
                with.Callback = new PeerClientToCollector(configuration, storage);
            });
        }

        public void Start(MetainfoFile metainfo)
        {
            Initialize(metainfo);
            Connect(metainfo);
        }

        private void Initialize(MetainfoFile metainfo)
        {
            storage.Register(metainfo, collector.CreateView());

            FileHash hash = metainfo.Data.Hash;
            ResourceRepository repository = storage.GetRepository(hash);

            Bitfield bitfield = repository.Initialize();
            ResourceRetriever retriever = storage.GetRetriever(hash);

            retriever.Initialize(bitfield);
            configuration.Callback.OnInitialized(metainfo.Data, new MetainfoSummary(bitfield));
        }

        private void Connect(MetainfoFile metainfo)
        {
            PeerConnector connector = new PeerConnector(with =>
            {
                with.Peer = configuration.Peer;
                with.Hash = metainfo.Data.Hash;
                with.Callback = collector.CreateConnectorCallback();
            });

            foreach (string tracker in metainfo.Trackers)
            {
                TrackerClient client = TrackerClientFactory.Create(tracker);
                TrackerAnnounce announce = client.Announce(with =>
                {
                    with.Peer = configuration.Peer;
                    with.Hash = metainfo.Data.Hash;
                });

                foreach (TrackerPeer peer in announce.Peers)
                {
                    connector.ConnectTo(peer.Host, peer.Port);
                }
            }
        }
    }
}