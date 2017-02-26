using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Leak.Completion;
using NUnit.Framework;

namespace Leak.Sockets.Tests
{
    public class UdpTests
    {
        [Test]
        public void CanTransferDataUsingUdpSockets()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                worker.Start();

                using (ManualResetEvent sync = new ManualResetEvent(false))
                {
                    IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, 44556);
                    byte[] data = Encoding.ASCII.GetBytes("abc");

                    SocketBuffer buffer = new SocketBuffer(data);
                    SocketFactory factory = new SocketFactory(worker);

                    using (UdpSocket sender = factory.Udp())
                    using (UdpSocket receiver = factory.Udp())
                    {
                        sender.Bind();
                        receiver.Bind(endpoint.Port);

                        receiver.Receive(new SocketBuffer(10), received =>
                        {
                            Assert.That(received.Status, Is.EqualTo(SocketStatus.OK));
                            Assert.That(received.Count, Is.EqualTo(3));
                            Assert.That(received.Buffer.Data.Take(3), Is.EqualTo(data));

                            sync.Set();
                        });

                        sender.Send(endpoint, buffer, sent =>
                        {
                            Assert.That(sent.Status, Is.EqualTo(SocketStatus.OK));
                            Assert.That(sent.Count, Is.EqualTo(3));
                        });
                    }

                    Assert.That(sync.WaitOne(200), Is.True);
                }
            }
        }
    }
}