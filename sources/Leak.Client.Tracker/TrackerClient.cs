using System;
using System.Threading.Tasks;
using Leak.Common;

namespace Leak.Client.Tracker
{
    public class TrackerClient : IDisposable
    {
        private readonly Runtime runtime;

        public TrackerClient()
        {
            runtime = new RuntimeInstance();
        }

        public Task<TrackerSession> ConnectAsync(string tracker)
        {
            runtime.Start();

            TrackerConnect connect = new TrackerConnect
            {
                Tracker = tracker,
                Localhost = PeerHash.Random(),
                Notifications = new NotificationCollection(),
                Completion = new TaskCompletionSource<TrackerSession>(),
                Pipeline = runtime.Pipeline,
                Worker = runtime.Worker
            };

            connect.Start();

            return connect.Completion.Task;
        }

        public void Dispose()
        {
            runtime.Stop();
        }
    }
}