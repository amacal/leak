using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Leak.Common;

namespace Leak.Client.Swarm
{
    public class SwarmClient : IDisposable
    {
        private readonly SwarmRuntime runtime;
        private readonly SwarmSettings settings;

        public SwarmClient()
        {
            runtime = new SwarmFactory();
            settings = new SwarmSettings();
        }

        public SwarmClient(SwarmSettings settings)
        {
            this.settings = settings;
            this.runtime = new SwarmFactory();
        }

        public Task<SwarmSession> ConnectAsync(FileHash hash, params string[] trackers)
        {
            runtime.Start();

            SwarmConnect connect = new SwarmConnect
            {
                Hash = hash,
                Settings = settings,
                Localhost = PeerHash.Random(),
                Notifications = new NotificationCollection(),
                Completion = new TaskCompletionSource<SwarmSession>(),
                Peers = new HashSet<PeerHash>(),
                Remotes = new HashSet<PeerAddress>(),
                Pipeline = runtime.Pipeline,
                Files = runtime.Files,
                Worker = runtime.Worker
            };

            connect.Start(trackers);
            return connect.Completion.Task;
        }

        public void Dispose()
        {
            runtime.Stop();
        }
    }
}