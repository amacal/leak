using System;
using System.Threading.Tasks;
using Leak.Common;
using Leak.Completion;
using Leak.Tasks;
using Leak.Tracker.Get;

namespace Leak.Client.Tracker
{
    public class TrackerClient
    {
        private readonly Uri address;

        public TrackerClient(Uri address)
        {
            this.address = address;
        }

        public Task<TrackerAnnounce> AnnounceAsync(FileHash hash)
        {
            LeakPipeline pipeline = new LeakPipeline();
            CompletionThread worker = new CompletionThread();

            Action complete = () => { };
            TaskCompletionSource<TrackerAnnounce> completion = new TaskCompletionSource<TrackerAnnounce>();

            TrackerGetHooks hooks = new TrackerGetHooks
            {
                OnAnnounced = data =>
                {
                    TrackerAnnounce announce = new TrackerAnnounce
                    {
                        Hash = data.Hash,
                        Interval = data.Interval,
                        Peers = data.Peers,
                        Leechers = data.Leechers,
                        Seeders = data.Seeders
                    };

                    complete.Invoke();
                    completion.SetResult(announce);
                },
                OnFailed = data =>
                {
                    complete.Invoke();
                    completion.SetException(new TrackerException(data.Reason));
                }
            };

            TrackerGetService service =
                new TrackerGetBuilder()
                    .WithPipeline(pipeline)
                    .WithWorker(worker)
                    .Build(hooks);

            complete = () =>
            {
                service.Stop();
                pipeline.Stop();
                worker.Dispose();
            };

            worker.Start();
            pipeline.Start();

            service.Start();
            service.Register(address, hash);

            return completion.Task;
        }
    }
}