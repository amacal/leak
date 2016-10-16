using Leak.Suckets;
using NUnit.Framework;

namespace Leak.Sockets.Tests
{
    public class CreateTests
    {
        [Test]
        public void CanCreateNewSocket()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                TcpSocketFactory factory = new TcpSocketFactory(worker);
                TcpSocket socket = factory.Create();

                Assert.That(socket, Is.Not.Null);
            }
        }

        [Test]
        public void CanDisposeNewSocket()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                TcpSocketFactory factory = new TcpSocketFactory(worker);
                TcpSocket socket = factory.Create();

                socket.Dispose();
            }
        }
    }
}