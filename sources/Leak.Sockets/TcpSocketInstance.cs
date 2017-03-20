using Leak.Completion;
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Leak.Sockets
{
    internal class TcpSocketInstance : TcpSocket
    {
        private readonly IntPtr handle;
        private readonly CompletionWorker worker;

        internal TcpSocketInstance(IntPtr handle, CompletionWorker worker)
        {
            this.handle = handle;
            this.worker = worker;
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

        public bool Bind(IPAddress address)
        {
            IPEndPoint endpoint = new IPEndPoint(address, 0);
            SocketBindRoutine routine = new SocketBindRoutine(endpoint);

            return routine.Execute(handle);
        }

        public TcpSocketInfo Info()
        {
            byte[] data = new byte[128];
            GCHandle pinned = GCHandle.Alloc(data, GCHandleType.Pinned);

            int length = data.Length;
            IntPtr pointer = Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);

            int result = TcpSocketInterop.getsockname(handle, pointer, ref length);
            uint error = TcpSocketInterop.GetLastError();

            pinned.Free();

            byte[] address = new byte[4];
            Array.Copy(data, 4, address, 0, 4);

            int port = data[2] * 256 + data[3];
            IPEndPoint endpoint = new IPEndPoint(new IPAddress(address), port);

            return new TcpSocketInfo(SocketStatus.OK, this, endpoint);
        }

        public void Listen(int backlog)
        {
            int value = TcpSocketInterop.listen(handle, backlog);

            if (value != 0)
                throw new Exception();

            worker.Add(handle);
        }

        public void Accept(TcpSocketAcceptCallback callback)
        {
            TcpSocketAcceptRoutine routine = new TcpSocketAcceptRoutine(handle, worker);
            TcpSocketAcceptResult result = new TcpSocketAcceptResult
            {
                Handle = handle,
                Socket = this,
                OnAccepted = callback
            };

            routine.Execute(result);
        }

        public Task<TcpSocketAccept> Accept()
        {
            TcpSocketAcceptRoutine routine = new TcpSocketAcceptRoutine(handle, worker);
            TcpSocketAcceptResult result = new TcpSocketAcceptResult
            {
                Handle = handle,
                Socket = this,
                Event = new ManualResetEvent(false),
            };

            Task<TcpSocketAccept> task = Task.Factory.FromAsync(result, result.Unpack);

            routine.Execute(result);
            return task;
        }

        public void Connect(IPEndPoint endpoint, TcpSocketConnectCallback callback)
        {
            TcpSocketConnectRoutine routine = new TcpSocketConnectRoutine(handle, worker, endpoint);
            TcpSocketConnectResult result = new TcpSocketConnectResult
            {
                Handle = handle,
                Socket = this,
                Endpoint = endpoint,
                OnConnected = callback
            };

            routine.Execute(result);
        }

        public Task<TcpSocketConnect> Connect(IPEndPoint endpoint)
        {
            TcpSocketConnectResult result = new TcpSocketConnectResult
            {
                Socket = this,
                Handle = handle,
                Endpoint = endpoint,
                Event = new ManualResetEvent(false)
            };

            TcpSocketConnectRoutine routine = new TcpSocketConnectRoutine(handle, worker, endpoint);
            Task<TcpSocketConnect> task = Task.Factory.FromAsync(result, result.Unpack);

            routine.Execute(result);
            return task;
        }

        public void Disconnect(TcpSocketDisconnectCallback callback)
        {
            TcpSocketDisconnectRoutine routine = new TcpSocketDisconnectRoutine(handle);
            TcpSocketDisconnectResult result = new TcpSocketDisconnectResult
            {
                Handle = handle,
                Socket = this,
                OnDisconnected = callback
            };

            routine.Execute(result);
        }

        public Task<TcpSocketDisconnect> Disconnect()
        {
            TcpSocketDisconnectResult result = new TcpSocketDisconnectResult
            {
                Handle = handle,
                Socket = this,
                Event = new ManualResetEvent(false)
            };

            Task<TcpSocketDisconnect> task = Task.Factory.FromAsync(result, ar => ((TcpSocketDisconnectResult)ar).CreateData());
            TcpSocketDisconnectRoutine routine = new TcpSocketDisconnectRoutine(handle);

            routine.Execute(result);
            return task;
        }

        public void Send(SocketBuffer buffer, TcpSocketSendCallback callback)
        {
            TcpSocketSendRoutine routine = new TcpSocketSendRoutine(handle, buffer);
            TcpSocketSendResult result = new TcpSocketSendResult
            {
                Socket = this,
                Buffer = buffer,
                OnSent = callback
            };

            routine.Execute(result);
        }

        public Task<TcpSocketSend> Send(SocketBuffer buffer)
        {
            TcpSocketSendResult result = new TcpSocketSendResult
            {
                Socket = this,
                Buffer = buffer,
                Event = new ManualResetEvent(false)
            };

            Task<TcpSocketSend> task = Task.Factory.FromAsync(result, ar => ((TcpSocketSendResult)ar).CreateData());
            TcpSocketSendRoutine routine = new TcpSocketSendRoutine(handle, buffer);

            routine.Execute(result);
            return task;
        }

        public void Receive(SocketBuffer buffer, TcpSocketReceiveCallback callback)
        {
            TcpSocketReceiveRoutine routine = new TcpSocketReceiveRoutine(handle, buffer);
            TcpSocketReceiveResult result = new TcpSocketReceiveResult
            {
                Socket = this,
                Buffer = buffer,
                OnReceived = callback
            };

            routine.Execute(result);
        }

        public Task<TcpSocketReceive> Receive(SocketBuffer buffer)
        {
            TcpSocketReceiveResult result = new TcpSocketReceiveResult
            {
                Socket = this,
                Buffer = buffer,
                Event = new ManualResetEvent(false)
            };

            Task<TcpSocketReceive> task = Task.Factory.FromAsync(result, ar => ((TcpSocketReceiveResult)ar).CreateData());
            TcpSocketReceiveRoutine routine = new TcpSocketReceiveRoutine(handle, buffer);

            routine.Execute(result);
            return task;
        }

        public void Dispose()
        {
            TcpSocketInterop.closesocket(handle);
        }
    }
}