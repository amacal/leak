using Leak.Sockets;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace Leak.Echo
{
    public class EchoBenchmark
    {
        private readonly TcpSocketFactory factory;
        private readonly IPEndPoint endpoint;

        public EchoBenchmark(TcpSocketFactory factory, IPEndPoint endpoint)
        {
            this.factory = factory;
            this.endpoint = endpoint;
        }

        public void Start(byte[] message, int count)
        {
            Stopwatch watch = Stopwatch.StartNew();

            for (int i = 0; i < count; i++)
            {
                EchoBenchmarkWorker worker = new EchoBenchmarkWorker(watch, factory, endpoint, message);

                worker.Start();
                Thread.Sleep(100);
            }
        }
    }
}