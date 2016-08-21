using Leak.Core.Battlefield;
using Leak.Core.Bouncer;
using Leak.Core.Cando;
using Leak.Core.Collector.Callbacks;
using Leak.Core.Communicator;
using Leak.Core.Congestion;
using Leak.Core.Infantry;
using Leak.Core.Loop;
using Leak.Core.Ranking;
using Leak.Core.Responder;
using System;

namespace Leak.Core.Collector
{
    public class PeerCollectorContext
    {
        private readonly object synchronized;
        private readonly PeerBouncerService bouncer;
        private readonly PeerCongestion congestion;
        private readonly InfantryService peers;
        private readonly ConnectionLoop loop;
        private readonly PeerCollectorConfiguration configuration;
        private readonly PeerCollectorStorage storage;
        private readonly CommunicatorService communicator;
        private readonly ResponderService responder;
        private readonly CandoService cando;
        private readonly RankingService ranking;
        private readonly BattlefieldService battlefield;

        public PeerCollectorContext(Action<PeerCollectorConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new PeerCollectorCallbackNothing();
            });

            peers = new InfantryService(with =>
            {
                with.Callback = new InfantryCallbackNothing();
            });

            congestion = new PeerCongestion(with =>
            {
                with.Callback = new PeerCongestionCallbackNothing();
            });

            loop = new ConnectionLoop(with =>
            {
                with.Callback = new PeerCollectorLoop(this);
            });

            bouncer = new PeerBouncerService(with =>
            {
                with.Callback = new PeerCollectorBouncer();
                with.Connections = 32;
            });

            communicator = new CommunicatorService(with =>
            {
            });

            responder = new ResponderService(with =>
            {
            });

            cando = new CandoService(with =>
            {
                with.Callback = new PeerCollectorCando(this);

                with.Extensions.Metadata(metadata =>
                {
                    metadata.Callback = new PeerCollectorMetadata(this);
                });

                with.Extensions.PeerExchange(exchange =>
                {
                    exchange.Callback = new PeerCollectorExchange(this);
                });
            });

            ranking = new RankingService(with =>
            {
            });

            battlefield = new BattlefieldService(with =>
            {
            });

            synchronized = new object();
            storage = new PeerCollectorStorage(configuration);
        }

        public object Synchronized
        {
            get { return synchronized; }
        }

        public PeerBouncerService Bouncer
        {
            get { return bouncer; }
        }

        public PeerCongestion Congestion
        {
            get { return congestion; }
        }

        public InfantryService Peers
        {
            get { return peers; }
        }

        public ConnectionLoop Loop
        {
            get { return loop; }
        }

        public PeerCollectorConfiguration Configuration
        {
            get { return configuration; }
        }

        public PeerCollectorCallback Callback
        {
            get { return configuration.Callback; }
        }

        public PeerCollectorStorage Storage
        {
            get { return storage; }
        }

        public CommunicatorService Communicator
        {
            get { return communicator; }
        }

        public ResponderService Responder
        {
            get { return responder; }
        }

        public CandoService Cando
        {
            get { return cando; }
        }

        public RankingService Ranking
        {
            get { return ranking; }
        }

        public BattlefieldService Battlefield
        {
            get { return battlefield; }
        }
    }
}