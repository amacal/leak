using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Leak.Completion;
using NUnit.Framework;

namespace Leak.Sockets.Tests
{
    public class ReceiveTests
    {
        [Test]
        public void CanReceiveDataUsingCallbackToExampleSite()
        {
            TimeSpan timeout = TimeSpan.FromSeconds(2);
            ManualResetEvent check = new ManualResetEvent(false);

            IPAddress[] addresses = Dns.GetHostAddresses("www.example.com");
            IPEndPoint endpoint = new IPEndPoint(addresses[0], 80);

            string request = "GET /index.html HTTP/1.1\r\nHost: www.example.com\r\n\r\n";
            byte[] data = Encoding.ASCII.GetBytes(request);

            using (CompletionThread worker = new CompletionThread())
            {
                TcpSocketFactory factory = new TcpSocketFactory(worker);
                TcpSocket socket = factory.Create();

                socket.Bind();
                worker.Start();

                TcpSocketReceiveCallback onReceived = result =>
                {
                    check.Set();
                };

                TcpSocketSendCallback onSent = result =>
                {
                    socket.Receive(data, onReceived);
                };

                TcpSocketConnectCallback onConnected = result =>
                {
                    socket.Send(data, onSent);
                };

                socket.Connect(endpoint, onConnected);
                bool completed = check.WaitOne(timeout);

                Assert.That(completed, Is.True);
            }
        }

        [Test]
        public async Task CanReceiveDataUsingTasksToExampleSite()
        {
            IPAddress[] addresses = Dns.GetHostAddresses("www.example.com");
            IPEndPoint endpoint = new IPEndPoint(addresses[0], 80);

            string request = "GET /index.html HTTP/1.1\r\nHost: www.example.com\r\n\r\n";
            byte[] data = Encoding.ASCII.GetBytes(request);

            using (CompletionThread worker = new CompletionThread())
            {
                TcpSocketFactory factory = new TcpSocketFactory(worker);
                TcpSocket socket = factory.Create();

                socket.Bind();
                worker.Start();

                await socket.Connect(endpoint);
                await socket.Send(data);
                await socket.Receive(data);
            }
        }

        [Test]
        public async Task CanReceiveDataUsingTasksToHostedEchoServer()
        {
            string request = "GET /index.html HTTP/1.1\r\nHost: www.example.com\r\n\r\n";
            byte[] input = Encoding.ASCII.GetBytes(request);
            byte[] output = new byte[input.Length];

            using (CompletionThread worker = new CompletionThread())
            {
                TcpSocketFactory factory = new TcpSocketFactory(worker);
                TcpSocket socket = factory.Create();

                using (EchoServer server = new EchoServer(factory))
                {
                    socket.Bind();
                    worker.Start();
                    server.Start();

                    await socket.Connect(server.Endpoint);
                    await socket.Send(input);
                    await socket.Receive(output);

                    Assert.That(input, Is.EqualTo(output));
                }
            }
        }

        [Test]
        public async Task CanReceiveDataUsingTasksToHostedEchoServerInTwoPieces()
        {
            string request = "GET /index.html HTTP/1.1\r\nHost: www.example.com\r\n\r\n";
            byte[] input = Encoding.ASCII.GetBytes(request);
            byte[] output = new byte[input.Length];

            byte[] part1 = new byte[input.Length / 2];
            byte[] part2 = new byte[input.Length - part1.Length];

            using (CompletionThread worker = new CompletionThread())
            {
                TcpSocketFactory factory = new TcpSocketFactory(worker);
                TcpSocket socket = factory.Create();

                using (EchoServer server = new EchoServer(factory))
                {
                    socket.Bind();
                    worker.Start();
                    server.Start();

                    await socket.Connect(server.Endpoint);
                    await socket.Send(input);

                    TcpSocketReceive received1 = await socket.Receive(part1);
                    TcpSocketReceive received2 = await socket.Receive(part2);

                    Array.Copy(part1, 0, output, 0, received1.Count);
                    Array.Copy(part2, 0, output, received1.Count, received2.Count);

                    Assert.That(input, Is.EqualTo(output));
                }
            }
        }

        [Test]
        public async Task CanHandleTerminatedStream()
        {
            IPAddress localhost = IPAddress.Loopback;
            IPEndPoint endpoint = new IPEndPoint(localhost, 1234);

            using (CompletionThread worker = new CompletionThread())
            {
                TcpSocketFactory factory = new TcpSocketFactory(worker);

                using (TcpSocket server = factory.Create())
                using (TcpSocket socket = factory.Create())
                {
                    socket.Bind();
                    worker.Start();

                    server.Bind(endpoint.Port);
                    server.Listen(1);

                    Task<TcpSocketAccept> acceptable = server.Accept();
                    await socket.Connect(endpoint);

                    TcpSocketAccept accepted = await acceptable;
                    accepted.Connection.Dispose();

                    byte[] buffer = new byte[10];
                    TcpSocketReceive received = await socket.Receive(buffer);

                    Assert.That(received.Status, Is.EqualTo(TcpSocketStatus.OK));
                    Assert.That(received.Count, Is.Zero);
                }
            }
        }

        [Test]
        public async Task CanHandleNotBoundSocket()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                TcpSocketFactory factory = new TcpSocketFactory(worker);

                using (TcpSocket socket = factory.Create())
                {
                    byte[] buffer = new byte[10];
                    TcpSocketReceive received = await socket.Receive(buffer);

                    Assert.That(received.Status, Is.Not.EqualTo(TcpSocketStatus.OK));
                }
            }
        }
    }
}