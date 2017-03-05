using System.Collections.Generic;
using System.Linq;
using Leak.Common;

namespace Leak.Tracker.Get
{
    public class TrackerGetUdpCollection : IEqualityComparer<byte[]>
    {
        private readonly Dictionary<byte[], TrackerGetUdpEntry> items;

        public TrackerGetUdpCollection()
        {
            items = new Dictionary<byte[], TrackerGetUdpEntry>(this);
        }

        public TrackerGetUdpEntry[] All()
        {
            return items.Values.ToArray();
        }

        public TrackerGetUdpEntry Add(byte[] transaction)
        {
            TrackerGetUdpEntry entry = new TrackerGetUdpEntry
            {
                Status = TrackerGetUdpStatus.Pending,
                Transaction = transaction,
            };

            items.Add(transaction, entry);
            return entry;
        }

        public TrackerGetUdpEntry Find(byte[] transaction)
        {
            TrackerGetUdpEntry entry;

            items.TryGetValue(transaction, out entry);
            return entry;
        }

        public void Remove(byte[] transaction)
        {
            items.Remove(transaction);
        }

        public void Clear()
        {
            items.Clear();
        }

        bool IEqualityComparer<byte[]>.Equals(byte[] x, byte[] y)
        {
            return Bytes.Equals(x, y);
        }

        int IEqualityComparer<byte[]>.GetHashCode(byte[] obj)
        {
            return obj[0];
        }
    }
}