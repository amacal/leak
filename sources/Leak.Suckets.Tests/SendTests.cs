using NUnit.Framework;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Leak.Suckets.Tests
{
    public class SendTests
    {
        [Test]
        public void CanSendDataUsingCallbackToExampleSite()
        {
            TimeSpan timeout = TimeSpan.FromSeconds(2);
            ManualResetEvent check = new ManualResetEvent(false);

            IPAddress[] addresses = Dns.GetHostAddresses("www.example.com");
            IPEndPoint endpoint = new IPEndPoint(addresses[0], 80);

            using (CompletionImpl worker = new CompletionImpl())
            {
                TcpSocketFactory factory = new TcpSocketFactory(worker);
                TcpSocket socket = factory.Create();

                socket.Bind();
                worker.Start();

                TcpSocketSendCallback onSent = result =>
                {
                    check.Set();
                };

                TcpSocketConnectCallback onConnected = result =>
                {
                    string request = "GET /index.html HTTP/1.1\r\nHost: www.example.com\r\n\r\n";
                    byte[] data = Encoding.ASCII.GetBytes(request);

                    socket.Send(data, onSent);
                };

                socket.Connect(endpoint, onConnected);
                bool completed = check.WaitOne(timeout);

                Assert.That(completed, Is.True);
            }
        }

        [Test]
        public async Task CanSendDataUsingAsyncToExampleSite()
        {
            IPAddress[] addresses = Dns.GetHostAddresses("www.example.com");
            IPEndPoint endpoint = new IPEndPoint(addresses[0], 80);

            string request = "GET /index.html HTTP/1.1\r\nHost: www.example.com\r\n\r\n";
            byte[] data = Encoding.ASCII.GetBytes(request);

            using (CompletionImpl worker = new CompletionImpl())
            {
                TcpSocketFactory factory = new TcpSocketFactory(worker);
                TcpSocket socket = factory.Create();

                socket.Bind();
                worker.Start();

                await socket.Connect(endpoint);
                await socket.Send(data);
            }
        }

        [Test]
        public async Task CanHandleTerminatedStream()
        {
            IPAddress localhost = IPAddress.Loopback;
            IPEndPoint endpoint = new IPEndPoint(localhost, 1234);

            using (CompletionImpl worker = new CompletionImpl())
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
                    TcpSocketSend sent = await socket.Send(buffer);

                    Assert.That(sent.Status, Is.EqualTo(TcpSocketStatus.OK));
                }
            }
        }

        [Test]
        public async Task CanHandleNotBoundSocket()
        {
            using (CompletionImpl worker = new CompletionImpl())
            {
                TcpSocketFactory factory = new TcpSocketFactory(worker);

                using (TcpSocket socket = factory.Create())
                {
                    byte[] buffer = new byte[10];
                    TcpSocketSend sent = await socket.Send(buffer);

                    Assert.That(sent.Status, Is.Not.EqualTo(TcpSocketStatus.OK));
                }
            }
        }
    }
}