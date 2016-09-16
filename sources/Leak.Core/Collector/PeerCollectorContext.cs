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
        private readonly CommunicatorService communicator;
        private readonly ResponderService responder;
        private readonly CandoService cando;
        private readonly RankingService ranking;
        private readonly BattlefieldService battlefield;
        private readonly PeerCollectorBlockFactory blockFactory;

        public PeerCollectorContext(Action<PeerCollectorConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new PeerCollectorCallbackNothing();
                with.Extensions = new PeerCollectorExtensionBuilder(this);
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
                with.Callback = new PeerCollectorToLoop(this);
            });

            bouncer = new PeerBouncerService(with =>
            {
                with.Callback = new PeerCollectorToBouncer();
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
                with.Callback = new PeerCollectorToCando(this);
                configuration.Extensions.Apply(with);
            });

            ranking = new RankingService(with =>
            {
                with.Minimum = -1024;
                with.Maximum = +4096;
            });

            battlefield = new BattlefieldService(with =>
            {
                with.Callback = new PeerCollectorToBattlefield(this);
            });

            synchronized = new object();
            blockFactory = new PeerCollectorBlockFactory();
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

        public PeerCollectorBlockFactory BlockFactory
        {
            get { return blockFactory; }
        }
    }
}