using Leak.Tasks;
using System;
using System.Collections.Generic;

namespace Leak.Networking
{
    public class NetworkPoolListener
    {
        private readonly NetworkPoolHooks hooks;
        private readonly Dictionary<long, NetworkPoolEntry> items;
        private readonly LeakQueue<NetworkPool> queue;

        public NetworkPoolListener(Dictionary<long, NetworkPoolEntry> items, LeakQueue<NetworkPool> queue, NetworkPoolHooks hooks)
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
                entry.Connection.Dispose();

                hooks.CallConnectionTerminated(entry.Connection);
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
                entry.Connection.Dispose();

                hooks.CallConnectionTerminated(entry.Connection);
            }
        }

        public void Schedule(LeakTask<NetworkPool> task)
        {
            queue.Add(task);
        }
    }
}