using Leak.Sockets;
using System;

namespace Leak.Echo
{
    public static class EchoServerStarter
    {
        public static void Start(EchoServerOptions options, TcpSocketFactory factory)
        {
            int port = options.Port ?? 7;
            EchoServer server = new EchoServer(factory, port);

            Console.WriteLine($"Starting echo server on port {port}.");
            server.Start();

            Console.ReadLine();
        }
    }
}