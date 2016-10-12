using NUnit.Framework;
using System.Threading.Tasks;

namespace Leak.Suckets.Tests
{
    public class EchoTests
    {
        [Test]
        public async Task CanHandleRequest()
        {
            using (CompletionImpl worker = new CompletionImpl())
            {
                TcpSocketFactory factory = new TcpSocketFactory(worker);
                EchoClient client = new EchoClient(factory);

                using (EchoServer server = new EchoServer(factory))
                {
                    worker.Start();
                    server.Start();

                    string payload = "abc";
                    string response = await client.Send(server.Endpoint, payload);

                    Assert.That(response, Is.EqualTo(payload));
                }
            }
        }

        [Test]
        public async Task CanHandleLongRequest()
        {
            using (CompletionImpl worker = new CompletionImpl())
            {
                TcpSocketFactory factory = new TcpSocketFactory(worker);
                EchoClient client = new EchoClient(factory);

                using (EchoServer server = new EchoServer(factory))
                {
                    worker.Start();
                    server.Start();

                    string payload = new string('a', 256 * 1024);
                    string response = await client.Send(server.Endpoint, payload);

                    Assert.That(response, Is.EqualTo(payload));
                }
            }
        }
    }
}