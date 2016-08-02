using Leak.Core.Collector;
using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Listener;
using Leak.Core.Messages;
using Leak.Core.Metadata;
using Leak.Core.Network;
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
        private readonly PeerListener listener;
        private readonly FileHashCollection hashes;
        private readonly NetworkPool pool;

        public PeerClient(Action<PeerClientConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Peer = PeerHash.Random();
                with.Destination = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.Create);
                with.Callback = new PeerClientCallbackNothing();
                with.Extensions = new PeerClientExtensionBuilder();
                with.Connector = new PeerClientConnectorBuilder();
                with.Listener = new PeerClientListenerBuilder();
            });

            storage = new PeerClientStorage(configuration);
            hashes = new FileHashCollection();
            callback = configuration.Callback;

            collector = new PeerCollector(with =>
            {
                with.Callback = new PeerClientToCollector(configuration, storage);
            });

            pool = new NetworkPool(with =>
            {
                with.Callback = collector.CreatePoolCallback();
            });

            if (configuration.Listener.Status == PeerClientListenerStatus.On)
            {
                listener = configuration.Listener.Build(with =>
                {
                    with.Callback = collector.CreateListenerCallback();
                    with.Peer = configuration.Peer;
                    with.Extensions = true;
                    with.Hashes = hashes;
                    with.Pool = pool;
                });

                listener.Start();
            }
        }

        public void Start(MetainfoFile metainfo)
        {
            Register(metainfo.Data);
            Schedule(metainfo.Data, metainfo.Trackers);
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
            string location = configuration.Destination;
            Metainfo metainfo = ResourceRepositoryToHash.Open(location, start.Hash);

            if (metainfo == null)
            {
                Register(start);
                Schedule(start);
            }
            else
            {
                Register(metainfo);
                Schedule(metainfo, start.Trackers.ToArray());
            }
        }

        private void Register(Metainfo metainfo)
        {
            storage.Register(metainfo, collector.CreateView());

            FileHash hash = metainfo.Hash;
            ResourceRepository repository = storage.GetRepository(hash);
            Bitfield bitfield = repository.Initialize();

            storage.WithBitfield(hash, bitfield);
            callback.OnInitialized(metainfo.Hash, new PeerClientMetainfo(bitfield));

            if (bitfield.Completed == bitfield.Length)
            {
                callback.OnCompleted(metainfo.Hash);
            }
        }

        private void Register(PeerClientStartConfiguration start)
        {
            storage.Register(start.Hash, collector.CreateView());
        }

        private void Schedule(Metainfo metainfo, string[] trackers)
        {
            PeerConnector connector = null;

            if (configuration.Connector.Status == PeerClientListenerStatus.On)
            {
                connector = configuration.Connector.Build(with =>
                {
                    with.Peer = configuration.Peer;
                    with.Hash = metainfo.Hash;
                    with.Callback = collector.CreateConnectorCallback();
                    with.Pool = pool;
                });
            }

            TrackerTelegraph telegraph = new TrackerTelegraph(with =>
            {
                if (connector != null)
                {
                    with.Callback = new PeerClientTelegraphConnect(configuration, metainfo.Hash, connector, storage);
                }
            });

            foreach (string tracker in trackers)
            {
                telegraph.Start(tracker, with =>
                {
                    with.Peer = configuration.Peer;
                    with.Hash = metainfo.Hash;
                });
            }

            hashes.Add(metainfo.Hash);
        }

        private void Schedule(PeerClientStartConfiguration start)
        {
            PeerConnector connector = null;

            if (configuration.Connector.Status == PeerClientListenerStatus.On)
            {
                connector = configuration.Connector.Build(with =>
                {
                    with.Hash = start.Hash;
                    with.Peer = configuration.Peer;
                    with.Callback = collector.CreateConnectorCallback();
                    with.Extensions = true;
                    with.Pool = pool;
                });
            }

            TrackerTelegraph telegraph = new TrackerTelegraph(with =>
            {
                if (connector != null)
                {
                    with.Callback = new PeerClientTelegraphConnect(configuration, start.Hash, connector, storage);
                }
            });

            foreach (string tracker in start.Trackers)
            {
                telegraph.Start(tracker, with =>
                {
                    with.Peer = configuration.Peer;
                    with.Hash = start.Hash;
                });
            }

            hashes.Add(start.Hash);
        }
    }
}