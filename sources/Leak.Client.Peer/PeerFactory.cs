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
        private LeakPipeline pipeline;
        private NetworkPool network;
        private CompletionThread worker;
        private FileFactory files;
        private MemoryService blocks;

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
                    pipeline = new LeakPipeline();
                    pipeline.Start();
                }

                if (worker == null)
                {
                    worker = new CompletionThread();
                    worker.Start();
                }

                if (blocks == null)
                {
                    blocks = new MemoryBuilder().Build();
                }

                if (network == null)
                {
                    network =
                        new NetworkPoolBuilder()
                            .WithPipeline(pipeline)
                            .WithWorker(worker)
                            .WithMemory(blocks.AsNetwork())
                            .Build(hooks);

                    network.Start();
                }

                if (files == null)
                {
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
                    network = null;
                }

                if (worker != null)
                {
                    worker.Dispose();
                    worker = null;
                }

                if (pipeline != null)
                {
                    pipeline?.Stop();
                    pipeline = null;
                }

                if (blocks != null)
                {
                    blocks = null;
                }
            }
        }
    }
}