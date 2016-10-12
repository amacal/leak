using NUnit.Framework;

namespace Leak.Suckets.Tests
{
    public class CreateTests
    {
        [Test]
        public void CanCreateNewSocket()
        {
            using (CompletionImpl worker = new CompletionImpl())
            {
                TcpSocketFactory factory = new TcpSocketFactory(worker);
                TcpSocket socket = factory.Create();

                Assert.That(socket, Is.Not.Null);
            }
        }

        [Test]
        public void CanDisposeNewSocket()
        {
            using (CompletionImpl worker = new CompletionImpl())
            {
                TcpSocketFactory factory = new TcpSocketFactory(worker);
                TcpSocket socket = factory.Create();

                socket.Dispose();
            }
        }
    }
}