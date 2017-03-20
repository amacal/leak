using System;
using System.Net;
using System.Threading.Tasks;
using Leak.Completion;

namespace Leak.Sockets
{
    internal class UdpSocketInstance : UdpSocket
    {
        private readonly IntPtr handle;
        private readonly CompletionWorker worker;

        internal UdpSocketInstance(IntPtr handle, CompletionWorker worker)
        {
            this.handle = handle;
            this.worker = worker;

            this.worker.Add(handle);
        }

        public bool Bind()
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
            SocketBindRoutine routine = new SocketBindRoutine(endpoint);

            return routine.Execute(handle);
        }

        public bool Bind(int port)
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port);
            SocketBindRoutine routine = new SocketBindRoutine(endpoint);

            return routine.Execute(handle);
        }

        public void SetTimeout(int seconds)
        {
            int value = seconds * 1000;
            SocketOptionRoutine routine = new SocketOptionRoutine(0xffff, 0x1006, value);

            routine.Execute(handle);
        }

        public void Send(IPEndPoint endpoint, SocketBuffer buffer, UdpSocketSendCallback callback)
        {
            UdpSocketSendRoutine routine = new UdpSocketSendRoutine(handle, endpoint, buffer);
            UdpSocketSendResult result = new UdpSocketSendResult
            {
                Socket = this,
                Buffer = buffer,
                OnSent = callback,
                Endpoint = endpoint
            };

            routine.Execute(result);
        }

        public Task<UdpSocketSend> Send(IPEndPoint endpoint, SocketBuffer buffer)
        {
            throw new NotImplementedException();
        }

        public void Receive(SocketBuffer buffer, UdpSocketReceiveCallback callback)
        {
            UdpSocketReceiveRoutine routine = new UdpSocketReceiveRoutine(handle, buffer);
            UdpSocketReceiveResult result = new UdpSocketReceiveResult
            {
                Socket = this,
                Buffer = buffer,
                OnReceived = callback
            };

            routine.Execute(result);
        }

        public Task<UdpSocketReceive> Receive(SocketBuffer buffer)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            UdpSocketInterop.closesocket(handle);
        }
    }
}