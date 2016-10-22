using Leak.Core.Network;
using NUnit.Framework;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Leak.Core.Tests.Integration.Network
{
    [TestFixture]
    public class NetworkPoolTests
    {
        [Test]
        public void CanEstablishConnection()
        {
            IPAddress address = Dns.GetHostAddresses("www.example.com")[0].MapToIPv4();
            IPEndPoint endpoint = new IPEndPoint(address, 80);

            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                ManualResetEvent completed = new ManualResetEvent(false);
                NetworkPool pool = new NetworkPool(with =>
                {
                });

                pool.Start();
                pool.Connect(socket, endpoint, connection =>
                {
                    completed.Set();
                });

                Assert.That(completed.WaitOne(5000), Is.True);
            }
        }
    }
}