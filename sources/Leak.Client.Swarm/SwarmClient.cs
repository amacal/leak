﻿using System;
using System.Collections.Concurrent;
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

        public SwarmClient()
        {
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
                Localhost = PeerHash.Random(),
                Notifications = new SwarmCollection(),
                Completion = new TaskCompletionSource<SwarmSession>(),
                Network = runtime.Network,
                Pipeline = runtime.Pipeline,
                Files = runtime.Files,
                Blocks = runtime.Blocks,
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