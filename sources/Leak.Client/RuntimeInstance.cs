using Leak.Completion;
using Leak.Files;
using Leak.Tasks;

namespace Leak.Client
{
    public class RuntimeInstance : Runtime
    {
        private LeakPipeline pipeline;
        private CompletionThread worker;
        private FileFactory files;

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
                    pipeline = new LeakPipeline();
                    pipeline.Start();
                }

                if (worker == null)
                {
                    worker = new CompletionThread();
                    worker.Start();
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
            }
        }
    }
}