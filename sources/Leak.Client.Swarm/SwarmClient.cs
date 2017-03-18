using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Leak.Common;
using Leak.Events;
using Leak.Networking;

namespace Leak.Client.Swarm
{
    public class SwarmClient : IDisposable
    {
        private readonly SwarmRuntime runtime;
        private readonly ConcurrentBag<SwarmConnect> online;
        private readonly SwarmSettings settings;

        public SwarmClient()
        {
            runtime = new SwarmFactory(null);
            online = new ConcurrentBag<SwarmConnect>();
            settings = new SwarmSettings();
        }

        public SwarmClient(SwarmSettings settings)
        {
            this.settings = settings;

            runtime = new SwarmFactory(null);
            online = new ConcurrentBag<SwarmConnect>();
        }

        public Task<SwarmSession> Connect(FileHash hash, params string[] trackers)
        {
            runtime.Start(new NetworkPoolHooks
            {
                OnConnectionTerminated = OnConnectionTerminated
            });

            SwarmConnect connect = new SwarmConnect
            {
                Hash = hash,
                Settings = settings,
                Localhost = PeerHash.Random(),
                Notifications = new SwarmCollection(),
                Completion = new TaskCompletionSource<SwarmSession>(),
                Peers = new HashSet<PeerHash>(),
                Remotes = new HashSet<PeerAddress>(),
                Network = runtime.Network,
                Pipeline = runtime.Pipeline,
                Files = runtime.Files,
                Worker = runtime.Worker
            };

            connect.Start(trackers);
            online.Add(connect);

            return connect.Completion.Task;
        }

        private void OnConnectionTerminated(ConnectionTerminated data)
        {
            foreach (SwarmConnect connect in online.ToArray())
            {
                if (connect.Glue?.Disconnect(data.Connection) == true)
                {
                    break;
                }
            }
        }

        public void Dispose()
        {
            runtime.Stop();
        }
    }
}