using Leak.Core.Client.Callbacks;
using Leak.Core.Client.Configuration;
using Leak.Core.Collector;
using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Listener;
using Leak.Core.Network;
using Leak.Core.Scheduler;
using Leak.Core.Telegraph;
using System;

namespace Leak.Core.Client
{
    public class PeerClientContext
    {
        private readonly PeerClientConfiguration configuration;
        private readonly PeerClientCollection collection;
        private readonly FileHashCollection hashes;
        private readonly SchedulerService scheduler;
        private readonly NetworkPool network;
        private readonly PeerCollector collector;
        private readonly PeerListener listener;
        private readonly TelegraphService telegraph;
        private readonly PeerConnector connector;

        public PeerClientContext(Action<PeerClientConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Peer = PeerHash.Random();
                with.Destination = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.Create);
                with.Callback = new PeerClientCallbackNothing();
                with.Connector = new PeerClientConnectorBuilder();
                with.Listener = new PeerClientListenerBuilder();
                with.Metadata = new PeerClientMetadataBuilder();
                with.PeerExchange = new PeerClientPeerExchangeBuilder();
            });

            collection = new PeerClientCollection();
            hashes = new FileHashCollection();

            collector = new PeerCollector(with =>
            {
                with.Callback = new PeerClientToCollector(this);

                configuration.Metadata.Apply(with);
                configuration.PeerExchange.Apply(with);
            });

            network = new NetworkPool(with =>
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
                    with.Pool = network;
                });
            }

            telegraph = new TelegraphService(with =>
            {
                with.Peer = configuration.Peer;
                with.Port = configuration.Listener.Port;
                with.Callback = new PeerClientToTelegraph(this);
            });

            scheduler = new SchedulerService(with =>
            {
                with.Collector = collector;
                with.Callback = new PeerClientToScheduler(this);
            });

            connector = new PeerConnector(with =>
            {
                with.Peer = configuration.Peer;
                with.Extensions = true;
                with.Pool = network;
                with.Callback = collector.CreateConnectorCallback();
            });

            connector.Start();
            listener.Start();
            telegraph.Start();
            network.Start();
        }

        /// <summary>
        /// The local peer hash.
        /// </summary>
        public PeerHash Peer
        {
            get { return configuration.Peer; }
        }

        public string Destination
        {
            get { return configuration.Destination; }
        }

        public PeerClientCallback Callback
        {
            get { return configuration.Callback; }
        }

        public SchedulerService Scheduler
        {
            get { return scheduler; }
        }

        public TelegraphService Telegraph
        {
            get { return telegraph; }
        }

        public NetworkPool Network
        {
            get { return network; }
        }

        public PeerCollector Collector
        {
            get { return collector; }
        }

        public PeerListener Listener
        {
            get { return listener; }
        }

        public PeerConnector Connector
        {
            get { return connector; }
        }
    }
}