using Leak.Tasks;
using System;
using System.Collections.Generic;

namespace Leak.Networking
{
    public class NetworkPoolListener
    {
        private readonly NetworkPoolConfiguration configuration;
        private readonly NetworkPoolDependencies dependencies;
        private readonly NetworkPoolHooks hooks;
        private readonly NetworkPoolQueue queue;
        private readonly Dictionary<long, NetworkPoolEntry> items;

        public NetworkPoolListener(Dictionary<long, NetworkPoolEntry> items, NetworkPoolQueue queue, NetworkPoolHooks hooks, NetworkPoolConfiguration configuration, NetworkPoolDependencies dependencies)
        {
            this.items = items;
            this.queue = queue;
            this.hooks = hooks;
            this.configuration = configuration;
            this.dependencies = dependencies;
        }

        public NetworkPoolMemoryBlock Allocate()
        {
            return dependencies.Memory.Allocate(configuration.BufferSize);
        }

        public bool IsAvailable(long id)
        {
            NetworkPoolEntry entry;

            lock (items)
            {
                items.TryGetValue(id, out entry);
            }

            return entry?.IsAvailable == true;
        }

        public void Disconnect(long id)
        {
            NetworkPoolEntry entry;

            lock (items)
            {
                items.TryGetValue(id, out entry);
            }

            if (entry != null)
            {
                entry.IsAvailable = false;
                entry.Connection?.Dispose();

                if (entry.Connection != null)
                {
                    hooks.CallConnectionTerminated(entry.Connection);
                    entry.Connection = null;
                }
            }
        }

        public void Fail(long id, Exception ex)
        {
            NetworkPoolEntry entry;

            lock (items)
            {
                items.TryGetValue(id, out entry);
            }

            if (entry != null)
            {
                entry.IsAvailable = false;
                entry.Connection?.Dispose();

                if (entry.Connection != null)
                {
                    hooks.CallConnectionTerminated(entry.Connection);
                    entry.Connection = null;
                }
            }
        }

        public void HandleSent(long identifier, int bytes)
        {
            NetworkPoolEntry entry;

            lock (items)
            {
                items.TryGetValue(identifier, out entry);
            }

            if (entry?.Connection != null)
            {
                hooks.CallConnectionSent(entry.Connection, bytes);
            }
        }

        public void HandleReceived(long identifier, int bytes)
        {
            NetworkPoolEntry entry;

            lock (items)
            {
                items.TryGetValue(identifier, out entry);
            }

            if (entry?.Connection != null)
            {
                hooks.CallConnectionReceived(entry.Connection, bytes);
            }
        }

        public void Schedule(NetworkPoolTask task)
        {
            queue.Add(task);
        }

        public void Schedule(Action callback)
        {
            queue.Add(new NetworkPoolInline(callback));
        }
    }
}