using System;
using System.Net;
using System.Net.Sockets;

namespace Leak.Core.Net
{
    public abstract class PeerSocket
    {
        public abstract void Bind(EndPoint localEP);

        public abstract void Listen(int backlog);

        public abstract IAsyncResult BeginConnect(string host, int port, AsyncCallback requestCallback, object state);

        public abstract void EndConnect(IAsyncResult asyncResult);

        public abstract IAsyncResult BeginAccept(AsyncCallback callback, object state);

        public abstract PeerSocket EndAccept(IAsyncResult asyncResult);

        public abstract void BeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state);

        public abstract int EndReceive(IAsyncResult asyncResult);

        public abstract int Send(byte[] buffer);
    }
}