using System.Collections.Generic;
using System.Linq;
using Leak.Sockets;

namespace Leak.Tracker.Get
{
    public class TrackerGetHttpCollection
    {
        private readonly Dictionary<TcpSocket, TrackerGetHttpEntry> items;

        public TrackerGetHttpCollection()
        {
            items = new Dictionary<TcpSocket, TrackerGetHttpEntry>();
        }

        public TrackerGetHttpEntry[] All()
        {
            return items.Values.ToArray();
        }

        public TrackerGetHttpEntry Add(TcpSocket socket)
        {
            TrackerGetHttpEntry entry = new TrackerGetHttpEntry
            {
                Status = TrackerGetHttpStatus.Pending,
                Socket = socket,
            };

            items.Add(socket, entry);
            return entry;
        }

        public TrackerGetHttpEntry Find(TcpSocket socket)
        {
            TrackerGetHttpEntry entry;

            items.TryGetValue(socket, out entry);
            return entry;
        }

        public void Remove(TcpSocket socket)
        {
            items.Remove(socket);
        }

        public void Clear()
        {
            items.Clear();
        }
    }
}