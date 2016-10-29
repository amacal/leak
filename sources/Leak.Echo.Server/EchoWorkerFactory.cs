using System.Collections.Generic;

namespace Leak.Echo.Server
{
    public class EchoWorkerFactory
    {
        private readonly Queue<EchoWorker> entries;

        public EchoWorkerFactory()
        {
            entries = new Queue<EchoWorker>();
        }

        public EchoWorker Next()
        {
            if (entries.Count == 0)
                return new EchoWorker(this);

            return entries.Dequeue();
        }

        public void Release(EchoWorker entry)
        {
            entries.Enqueue(entry);
        }
    }
}