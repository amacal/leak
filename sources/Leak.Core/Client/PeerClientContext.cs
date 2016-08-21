using Leak.Core.Collector;
using Leak.Core.Common;
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
        private readonly TrackerTelegraph telegraph;

        public PeerClientContext(Action<PeerClientConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Peer = PeerHash.Random();
                with.Destination = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.Create);
                with.Callback = new PeerClientCallbackNothing();
                with.Connector = new PeerClientConnectorBuilder();
                with.Listener = new PeerClientListenerBuilder();
            });

            collection = new PeerClientCollection();
            hashes = new FileHashCollection();

            collector = new PeerCollector(with =>
            {
                with.Callback = new PeerClientToCollector(this);
            });

            network = new NetworkPool(with =>
            {
                with.Callback = collector.CreatePoolCallback();
            });

            listener = configuration.Listener.Build(with =>
            {
                with.Callback = collector.CreateListenerCallback();
                with.Peer = configuration.Peer;
                with.Extensions = true;
                with.Hashes = hashes;
                with.Pool = network;
            });

            telegraph = new TrackerTelegraph(with =>
            {
                with.Callback = new PeerClientToTelegraph(this);
            });

            scheduler = new SchedulerService(with =>
            {
                with.Collector = collector;
                with.Callback = new PeerClientToScheduler(this);
            });
        }

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

        public TrackerTelegraph Telegraph
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
    }
}