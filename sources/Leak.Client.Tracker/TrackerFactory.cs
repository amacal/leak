using Leak.Completion;
using Leak.Tasks;
using Leak.Tracker.Get;

namespace Leak.Client.Tracker
{
    public class TrackerFactory : TrackerRuntime
    {
        private LeakPipeline pipeline;
        private CompletionThread worker;
        private TrackerGetService service;

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
                    pipeline = new LeakPipeline();
                    pipeline.Start();
                }

                if (worker == null)
                {
                    worker = new CompletionThread();
                    worker.Start();
                }

                if (service == null)
                {
                    service =
                        new TrackerGetBuilder()
                            .WithPipeline(pipeline)
                            .WithWorker(worker)
                            .Build(hooks);

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
                    service.Dispose();
                    service = null;
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
            }
        }
    }
}