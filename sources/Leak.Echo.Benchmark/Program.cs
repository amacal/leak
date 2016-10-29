using Leak.Sockets;
using Leak.Suckets;
using System;
using System.Net;

namespace Leak.Echo.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (CompletionThread worker = new CompletionThread())
            {
                TcpSocketFactory factory = new TcpSocketFactory(worker);
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(args[0]), 3000);

                worker.Start();

                Random random = new Random();
                EchoBenchmark benchmark = new EchoBenchmark(factory, endpoint);

                byte[] message = new byte[1024];
                random.NextBytes(message);

                benchmark.Start(message, 17);
                Console.ReadLine();
            }
        }
    }
}