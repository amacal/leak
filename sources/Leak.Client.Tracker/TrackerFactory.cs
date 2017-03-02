using Leak.Completion;
using Leak.Tasks;
using Leak.Tracker.Get;

namespace Leak.Client.Tracker
{
    public class TrackerFactory : TrackerRuntime
    {
        private readonly TrackerLogger logger;

        private LeakPipeline pipeline;
        private CompletionThread worker;
        private TrackerGetService service;

        public TrackerFactory(TrackerLogger logger)
        {
            this.logger = logger;
        }

        public TrackerGetService Service
        {
            get { return service; }
        }

        public void Start(TrackerGetHooks hooks)
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

                if (service == null)
                {
                    logger?.Info("creating tracker-get service");

                    service =
                        new TrackerGetBuilder()
                            .WithPipeline(pipeline)
                            .WithWorker(worker)
                            .Build(hooks);

                    logger?.Info("starting tracker-get service");
                    service.Start();
                }
            }
        }

        public void Stop()
        {
            lock (this)
            {
                if (service != null)
                {
                    logger?.Info("disposing tracker-get-service");
                    service.Dispose();
                    service = null;
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