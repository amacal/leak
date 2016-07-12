using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Leak.Core.Tests
{
    public class SocketContainer : IDisposable
    {
        private readonly Dictionary<string, Socket> actives;
        private readonly Dictionary<string, Socket> passives;
        private readonly Dictionary<string, Socket> connected;

        public SocketContainer()
        {
            actives = new Dictionary<string, Socket>();
            passives = new Dictionary<string, Socket>();
            connected = new Dictionary<string, Socket>();
        }

        public Socket this[string name]
        {
            get
            {
                Socket value;

                if (actives.TryGetValue(name, out value))
                    return value;

                if (connected.TryGetValue(name, out value))
                    return value;

                throw new InvalidOperationException();
            }
        }

        public void Active(string name)
        {
            actives.Add(name, new Socket(SocketType.Stream, ProtocolType.Tcp));
        }

        public void Passive(string name)
        {
            passives.Add(name, new Socket(SocketType.Stream, ProtocolType.Tcp));
        }

        public void Connect(string active, string passive)
        {
            passives[passive].Bind(new IPEndPoint(IPAddress.Loopback, 8080));
            passives[passive].Listen(1);

            actives[active].Connect(new IPEndPoint(IPAddress.Loopback, 8080));
            connected.Add(passive, passives[passive].Accept());
        }

        public void Dispose()
        {
            foreach (Socket socket in actives.Values)
            {
                socket.Dispose();
            }

            foreach (Socket socket in passives.Values)
            {
                socket.Dispose();
            }
        }
    }
}