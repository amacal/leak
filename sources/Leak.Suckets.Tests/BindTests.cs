using NUnit.Framework;

namespace Leak.Suckets.Tests
{
    public class BindTests
    {
        [Test]
        public void CanBindSocket()
        {
            using (CompletionImpl worker = new CompletionImpl())
            {
                TcpSocketFactory factory = new TcpSocketFactory(worker);
                TcpSocket socket = factory.Create();

                socket.Bind();
            }
        }
    }
}