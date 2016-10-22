using System.Collections.Concurrent;

namespace Leak.Core.Connector
{
    public class PeerConnectorQueue
    {
        private readonly ConcurrentQueue<PeerConnectorTask> items;

        public PeerConnectorQueue()
        {
            items = new ConcurrentQueue<PeerConnectorTask>();
        }

        public void Add(PeerConnectorTask task)
        {
            items.Enqueue(task);
        }

        public void Clear()
        {
            PeerConnectorTask task;

            while (items.TryDequeue(out task))
            {
            }
        }

        public void Process()
        {
            PeerConnectorTask task;

            while (items.TryDequeue(out task))
            {
                task.Execute();
            }
        }
    }
}