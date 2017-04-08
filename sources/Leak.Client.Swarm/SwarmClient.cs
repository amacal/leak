using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Client.Swarm
{
    public class SwarmClient : IDisposable
    {
        private readonly Runtime runtime;
        private readonly SwarmSettings settings;

        public SwarmClient()
        {
            settings = new SwarmSettings();
            runtime = new RuntimeInstance();
        }

        public SwarmClient(SwarmSettings settings)
        {
            this.settings = settings;
            this.runtime = new RuntimeInstance();
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
                Remotes = new HashSet<NetworkAddress>(),
                Pipeline = runtime.Pipeline,
                Files = runtime.Files,
                Worker = runtime.Worker
            };

            connect.Start();
            connect.Announce(trackers);

            return connect.Completion.Task;
        }

        public void Dispose()
        {
            runtime.Stop();
        }
    }
}