using Leak.Common;
using Leak.Completion;
using Leak.Files;
using Leak.Memory;
using Leak.Networking;
using Leak.Tasks;

namespace Leak.Client.Peer
{
    public class PeerFactory : PeerRuntime
    {
        private readonly PeerLogger logger;

        private LeakPipeline pipeline;
        private NetworkPool network;
        private CompletionThread worker;
        private FileFactory files;
        private MemoryService blocks;

        public PeerFactory(PeerLogger logger)
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

        public FileFactory Files
        {
            get { return files; }
        }

        public DataBlockFactory Blocks
        {
            get { return blocks; }
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

                if (blocks == null)
                {
                    logger?.Info("creating blocks factory");
                    blocks = new MemoryBuilder().Build();
                }

                if (network == null)
                {
                    logger?.Info("creating network pool");

                    network =
                        new NetworkPoolBuilder()
                            .WithPipeline(pipeline)
                            .WithWorker(worker)
                            .WithMemory(blocks.AsNetwork())
                            .Build(hooks);

                    logger?.Info("starting network pool");
                    network.Start();
                }

                if (files == null)
                {
                    logger?.Info("creating file factory");
                    files = new FileFactory(worker);
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

                if (blocks != null)
                {
                    logger?.Info("disposing blocks factory");
                    blocks = null;
                }
            }
        }
    }
}