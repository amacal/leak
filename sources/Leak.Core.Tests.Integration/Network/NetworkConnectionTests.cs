using FluentAssertions;
using Leak.Core.Network;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Leak.Core.Tests.Integration.Network
{
    [TestFixture]
    public class NetworkConnectionTests
    {
        public const string Request = "GET /index.html HTTP/1.1\r\nHost: www.example.com\r\n\r\n";

        [Test]
        public void CanReceiveData()
        {
            using (Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Connect("www.example.com", 80);

                NetworkPool pool = new NetworkPool();
                IncomingMessageHandler handler = new IncomingMessageHandler();
                NetworkConnection connection = pool.Create(socket, NetworkDirection.Outgoing);

                connection.Direction.Should().Be(NetworkDirection.Outgoing);
                connection.Remote.Should().Be("93.184.216.34");

                connection.Send(new NetworkOutgoingMessageBytes(System.Text.Encoding.ASCII.GetBytes(Request)));
                connection.Receive(handler);

                handler.Ready.WaitOne(TimeSpan.FromMinutes(1));

                handler.Messages.Should().HaveCount(1);
                handler.ToString().Should().Contain("Content-Type: text/html");
                handler.ToString().Should().Contain("</html>");
            }
        }

        private class IncomingMessageHandler : NetworkIncomingMessageHandler
        {
            private readonly List<NetworkIncomingMessage> messages;
            private readonly ManualResetEvent onReady;

            public IncomingMessageHandler()
            {
                messages = new List<NetworkIncomingMessage>();
                onReady = new ManualResetEvent(false);
            }

            public IEnumerable<NetworkIncomingMessage> Messages
            {
                get { return messages; }
            }

            public WaitHandle Ready
            {
                get { return onReady; }
            }

            public override string ToString()
            {
                byte[] data = new byte[0];

                foreach (NetworkIncomingMessage message in messages)
                {
                    Array.Resize(ref data, message.Length + data.Length);
                    Array.Copy(message.ToBytes(), 0, data, data.Length - message.Length, message.Length);
                }

                return System.Text.Encoding.UTF8.GetString(data);
            }

            public void OnMessage(NetworkIncomingMessage message)
            {
                if (message.Length == 1614)
                {
                    messages.Add(message);
                    onReady.Set();
                }
                else
                {
                    message.Continue(this);
                }
            }

            public void OnException(Exception ex)
            {
                Assert.Fail();
            }

            public void OnDisconnected()
            {
                Assert.Fail();
            }
        }
    }
}