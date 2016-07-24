using Leak.Core.Collector;
using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Messages;
using Leak.Core.Metadata;
using Leak.Core.Repository;
using Leak.Core.Telegraph;
using System;
using System.Collections.Generic;

namespace Leak.Core.Client
{
    public class PeerClient
    {
        private readonly PeerCollector collector;
        private readonly PeerClientStorage storage;
        private readonly PeerClientConfiguration configuration;
        private readonly PeerClientCallback callback;

        public PeerClient(Action<PeerClientConfiguration> configurer)
        {
            this.configuration = configurer.Configure(with =>
            {
                with.Peer = PeerHash.Random();
                with.Destination = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.Create);
                with.Callback = new PeerClientCallbackToNothing();
                with.Extensions = new PeerClientExtensionBuilder();
            });

            this.storage = new PeerClientStorage(configuration);
            this.callback = configuration.Callback;

            this.collector = new PeerCollector(with =>
            {
                with.Callback = new PeerClientToCollector(configuration, storage);
            });
        }

        public void Start(MetainfoFile metainfo)
        {
            Initialize(metainfo);
            Schedule(metainfo);
        }

        public void Start(Action<PeerClientStartConfiguration> configurer)
        {
            Start(configurer.Configure(with =>
            {
                with.Trackers = new List<string>();
            }));
        }

        private void Start(PeerClientStartConfiguration start)
        {
            Register(start);
            Schedule(start);
        }

        private void Initialize(MetainfoFile metainfo)
        {
            storage.Register(metainfo.Data, collector.CreateView());

            FileHash hash = metainfo.Data.Hash;
            ResourceRepository repository = storage.GetRepository(hash);
            Bitfield bitfield = repository.Initialize();

            storage.WithBitfield(hash, bitfield);
            callback.OnInitialized(metainfo.Data.Hash, new PeerClientMetainfoSummary(bitfield));
        }

        private void Register(PeerClientStartConfiguration start)
        {
            storage.Register(start.Hash, collector.CreateView());
        }

        private void Schedule(MetainfoFile metainfo)
        {
            PeerConnector connector = new PeerConnector(with =>
            {
                with.Peer = configuration.Peer;
                with.Hash = metainfo.Data.Hash;
                with.Callback = collector.CreateConnectorCallback();
            });

            TrackerTelegraph telegraph = new TrackerTelegraph(with =>
            {
                with.Callback = new PeerClientToTelegraph(configuration, metainfo.Data.Hash, connector, storage);
            });

            foreach (string tracker in metainfo.Trackers)
            {
                telegraph.Start(tracker, with =>
                {
                    with.Peer = configuration.Peer;
                    with.Hash = metainfo.Data.Hash;
                });
            }
        }

        private void Schedule(PeerClientStartConfiguration start)
        {
            PeerConnector connector = new PeerConnector(with =>
            {
                with.Hash = start.Hash;
                with.Peer = configuration.Peer;
                with.Callback = collector.CreateConnectorCallback();
                with.Extensions = true;
            });

            TrackerTelegraph telegraph = new TrackerTelegraph(with =>
            {
                with.Callback = new PeerClientToTelegraph(configuration, start.Hash, connector, storage);
            });

            foreach (string tracker in start.Trackers)
            {
                telegraph.Start(tracker, with =>
                {
                    with.Peer = configuration.Peer;
                    with.Hash = start.Hash;
                });
            }
        }
    }
}