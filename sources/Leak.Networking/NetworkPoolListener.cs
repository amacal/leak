using Leak.Tasks;
using System;
using System.Collections.Generic;

namespace Leak.Networking
{
    public class NetworkPoolListener
    {
        private readonly NetworkPoolHooks hooks;
        private readonly LeakQueue<NetworkPoolInstance> queue;
        private readonly Dictionary<long, NetworkPoolEntry> items;

        public NetworkPoolListener(Dictionary<long, NetworkPoolEntry> items, LeakQueue<NetworkPoolInstance> queue, NetworkPoolHooks hooks)
        {
            this.items = items;
            this.queue = queue;
            this.hooks = hooks;
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

        public void Schedule(LeakTask<NetworkPoolInstance> task)
        {
            queue.Add(task);
        }
    }
}