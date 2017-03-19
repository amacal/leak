using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Leak.Common;

namespace Leak.Memory
{
    public class MemoryCollection
    {
        private readonly MemoryContext context;
        private readonly object synchronizer;

        private readonly SortedDictionary<int, ConcurrentQueue<byte[]>> items;

        private int count;
        private Size allocation;

        public MemoryCollection(MemoryContext context)
        {
            this.count = 0;
            this.allocation = new Size(0);
            this.synchronizer = new object();

            this.context = context;
            this.items = new SortedDictionary<int, ConcurrentQueue<byte[]>>();

            if (context.Configuration.Thresholds != null)
            {
                foreach (int threshold in context.Configuration.Thresholds)
                {
                    items.Add(threshold, new ConcurrentQueue<byte[]>());
                }
            }

            if (items.ContainsKey(context.Configuration.MaxBlockSize) == false)
            {
                items.Add(context.Configuration.MaxBlockSize, new ConcurrentQueue<byte[]>());
            }
        }

        public MemoryBlock Allocate(int size)
        {
            byte[] data = null;

            if (size > context.Configuration.MaxBlockSize)
            {
                throw new InvalidOperationException();
            }

            foreach (int key in items.Keys)
            {
                if (size <= key)
                {
                    if (items[key].TryDequeue(out data) == false)
                    {
                        data = new byte[key];

                        lock (synchronizer)
                        {
                            count = count + 1;
                            allocation = allocation.Increase(data.Length);
                        }

                        context.CallSnapshot(count, allocation);
                    }

                    break;
                }
            }

            return new MemoryBlock(data, 0, size, this);
        }

        public void Release(byte[] data)
        {
            foreach (int key in items.Keys)
            {
                if (data.Length == key)
                {
                    items[key].Enqueue(data);
                    break;
                }
            }
        }
    }
}