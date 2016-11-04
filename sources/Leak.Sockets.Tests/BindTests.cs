using Leak.Completion;
using NUnit.Framework;

namespace Leak.Sockets.Tests
{
    public class BindTests
    {
        [Test]
        public void CanBindSocket()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                TcpSocketFactory factory = new TcpSocketFactory(worker);
                TcpSocket socket = factory.Create();

                socket.Bind();
            }
        }
    }
}