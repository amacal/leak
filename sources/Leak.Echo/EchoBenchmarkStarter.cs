using Leak.Sockets;
using System;
using System.Net;

namespace Leak.Echo
{
    public static class EchoBenchmarkStarter
    {
        public static void Start(EchoBenchmarkOptions options, SocketFactory factory)
        {
            int port = options.Port ?? 7;
            string host = options.Host ?? "127.0.0.1";

            int size = options.Size ?? 1024;
            int workers = options.Workers ?? 8;

            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(host), port);
            EchoBenchmark benchmark = new EchoBenchmark(factory, endpoint);

            Random random = new Random();
            byte[] message = new byte[size];

            random.NextBytes(message);

            Console.WriteLine($"Starting benchmark using {workers} workers and {size} bytes messages.");
            Console.WriteLine($"Connecting to {host}:{port}.");

            benchmark.Start(message, workers);
            Console.ReadLine();
        }
    }
}