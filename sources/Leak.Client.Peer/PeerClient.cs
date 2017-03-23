using Leak.Common;
using System;
using System.Threading.Tasks;

namespace Leak.Client.Peer
{
    public class PeerClient : IDisposable
    {
        private readonly Runtime runtime;

        public PeerClient()
        {
            runtime = new RuntimeInstance();
        }

        public Task<PeerSession> Connect(FileHash hash, PeerAddress remote)
        {
            runtime.Start();

            PeerConnect connect = new PeerConnect
            {
                Hash = hash,
                Address = remote,
                Localhost = PeerHash.Random(),
                Notifications = new NotificationCollection(),
                Completion = new TaskCompletionSource<PeerSession>(),
                Pipeline = runtime.Pipeline,
                Files = runtime.Files,
                Worker = runtime.Worker
            };

            connect.Start();
            connect.Connect(remote);

            return connect.Completion.Task;
        }

        public void Dispose()
        {
            runtime.Stop();
        }
    }
}