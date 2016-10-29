using Leak.Sockets;
using Leak.Suckets;
using System;

namespace Leak.Echo.Server
{
    public static class Program
    {
        public static void Main()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                TcpSocketFactory factory = new TcpSocketFactory(worker);

                worker.Start();

                using (EchoServer server = new EchoServer(factory, 3000))
                {
                    server.Start();
                    Console.ReadLine();
                }
            }
        }
    }
}