using Leak.Completion;
using Leak.Networking;
using Leak.Tasks;

namespace Leak.Client.Swarm
{
    public class PeerFactory : SwarmRuntime
    {
        private readonly SwarmLogger logger;

        private LeakPipeline pipeline;
        private NetworkPool network;
        private CompletionThread worker;

        public PeerFactory(SwarmLogger logger)
        {
            this.logger = logger;
        }

        public PipelineService Pipeline
        {
            get { return pipeline; }
        }

        public NetworkPool Network
        {
            get { return network; }
        }

        public void Start(NetworkPoolHooks hooks)
        {
            lock (this)
            {
                if (pipeline == null)
                {
                    logger?.Info("creating pipeline service");
                    pipeline = new LeakPipeline();

                    logger?.Info("starting pipeline service");
                    pipeline.Start();
                }

                if (worker == null)
                {
                    logger?.Info("creating completion service");
                    worker = new CompletionThread();

                    logger?.Info("starting completion service");
                    worker.Start();
                }

                if (network == null)
                {
                    logger?.Info("creating network pool");

                    network =
                        new NetworkPoolInstance(new NetworkPoolDependency
                        {
                            Pipeline = pipeline,
                            Completion = worker
                        }, hooks);

                    logger?.Info("starting network pool");
                    network.Start();
                }
            }
        }

        public void Stop()
        {
            lock (this)
            {
                if (network != null)
                {
                    logger?.Info("disposing network pool");
                    network = null;
                }

                if (worker != null)
                {
                    logger?.Info("disposing completion service");
                    worker.Dispose();
                    worker = null;
                }

                if (pipeline != null)
                {
                    logger?.Info("disposing pipeline service");
                    pipeline?.Stop();
                    pipeline = null;
                }
            }
        }
    }
}