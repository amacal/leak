using System.Collections.Generic;

namespace Leak.Echo
{
    public class EchoServerWorkerFactory
    {
        private readonly Queue<EchoServerWorker> entries;

        public EchoServerWorkerFactory()
        {
            entries = new Queue<EchoServerWorker>();
        }

        public EchoServerWorker Next()
        {
            if (entries.Count == 0)
                return new EchoServerWorker(this);

            return entries.Dequeue();
        }

        public void Release(EchoServerWorker entry)
        {
            entries.Enqueue(entry);
        }
    }
}