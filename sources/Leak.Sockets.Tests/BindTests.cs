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
                SocketFactory factory = new SocketFactory(worker);
                TcpSocket socket = factory.Tcp();

                socket.Bind();
            }
        }
    }
}