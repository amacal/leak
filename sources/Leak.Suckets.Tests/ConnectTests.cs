using NUnit.Framework;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Leak.Suckets.Tests
{
    public class ConnectTests
    {
        [Test]
        public void CanConnectUsingCallbackToExampleSite()
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

                TcpSocketConnectCallback onConnected = result =>
                {
                    check.Set();
                };

                socket.Connect(endpoint, onConnected);
                bool completed = check.WaitOne(timeout);

                Assert.That(completed, Is.True);
            }
        }

        [Test]
        public async Task CanConnectUsingTasksToExampleSite()
        {
            IPAddress[] addresses = Dns.GetHostAddresses("www.example.com");
            IPEndPoint endpoint = new IPEndPoint(addresses[0], 80);

            using (CompletionImpl worker = new CompletionImpl())
            {
                TcpSocketFactory factory = new TcpSocketFactory(worker);
                TcpSocket socket = factory.Create();

                socket.Bind();
                worker.Start();

                TcpSocketConnect connect = await socket.Connect(endpoint);
                TcpSocketStatus expected = TcpSocketStatus.OK;

                Assert.That(connect.Status, Is.EqualTo(expected));
            }
        }

        [Test]
        public async Task CanConnectUsingTasksToNotAvailableSite()
        {
            IPAddress[] addresses = Dns.GetHostAddresses("www.example.com");
            IPEndPoint endpoint = new IPEndPoint(addresses[0], 8123);

            using (CompletionImpl worker = new CompletionImpl())
            {
                TcpSocketFactory factory = new TcpSocketFactory(worker);
                TcpSocket socket = factory.Create();

                socket.Bind();
                worker.Start();

                TcpSocketConnect connect = await socket.Connect(endpoint);
                TcpSocketStatus expected = TcpSocketStatus.TimedOut;

                Assert.That(connect.Status, Is.EqualTo(expected));
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
                    IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, 80);
                    TcpSocketConnect connected = await socket.Connect(endpoint);

                    Assert.That(connected.Status, Is.Not.EqualTo(TcpSocketStatus.OK));
                }
            }
        }
    }
}