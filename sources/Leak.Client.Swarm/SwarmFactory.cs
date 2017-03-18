using Leak.Completion;
using Leak.Files;
using Leak.Networking;
using Leak.Tasks;

namespace Leak.Client.Swarm
{
    public class SwarmFactory : SwarmRuntime
    {
        private readonly SwarmLogger logger;

        private LeakPipeline pipeline;
        private CompletionThread worker;
        private FileFactory files;

        public SwarmFactory(SwarmLogger logger)
        {
            this.logger = logger;
        }

        public PipelineService Pipeline
        {
            get { return pipeline; }
        }

        public FileFactory Files
        {
            get { return files; }
        }

        public CompletionWorker Worker
        {
            get { return worker; }
        }

        public void Start()
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