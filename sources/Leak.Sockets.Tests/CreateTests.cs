using Leak.Completion;
using NUnit.Framework;

namespace Leak.Sockets.Tests
{
    public class CreateTests
    {
        [Test]
        public void CanCreateNewTcpSocket()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                SocketFactory factory = new SocketFactory(worker);
                TcpSocket socket = factory.Tcp();

                Assert.That(socket, Is.Not.Null);
            }
        }

        [Test]
        public void CanDisposeNewTcpSocket()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                SocketFactory factory = new SocketFactory(worker);
                TcpSocket socket = factory.Tcp();

                socket.Dispose();
            }
        }

        [Test]
        public void CanCreateNewUdpSocket()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                SocketFactory factory = new SocketFactory(worker);
                UdpSocket socket = factory.Udp();

                Assert.That(socket, Is.Not.Null);
            }
        }

        [Test]
        public void CanDisposeNewUdpSocket()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                SocketFactory factory = new SocketFactory(worker);
                UdpSocket socket = factory.Udp();

                socket.Dispose();
            }
        }
    }
}