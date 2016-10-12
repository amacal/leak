using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Leak.Suckets.Tests
{
    public class EchoClient
    {
        private readonly TcpSocketFactory factory;

        public EchoClient(TcpSocketFactory factory)
        {
            this.factory = factory;
        }

        public async Task<string> Send(IPEndPoint endpoint, string message)
        {
            int progress = 0;
            byte[] output = new byte[message.Length];
            byte[] bytes = Encoding.ASCII.GetBytes(message);

            using (TcpSocket socket = factory.Create())
            {
                socket.Bind();

                TcpSocketConnect connected = await socket.Connect(endpoint);
                TcpSocketSend sent = await socket.Send(bytes);

                while (progress < output.Length)
                {
                    TcpSocketBuffer buffer = new TcpSocketBuffer(output, progress);
                    TcpSocketReceive received = await socket.Receive(buffer);

                    if (received.Count == 0)
                        break;

                    progress += received.Count;
                }
            }

            return Encoding.ASCII.GetString(output);
        }
    }
}