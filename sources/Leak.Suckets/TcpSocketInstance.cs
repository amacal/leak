using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Leak.Suckets
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

        public void Bind()
        {
            byte[] data = { 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            GCHandle pinned = GCHandle.Alloc(data, GCHandleType.Pinned);

            IntPtr address = Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);
            int value = TcpSocketInterop.bind(handle, address, data.Length);

            pinned.Free();

            if (value != 0)
                throw new Exception();
        }

        public void Bind(int port)
        {
            byte[] data = { 0x02, 0x00, (byte)(port / 256), (byte)(port % 256), 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            GCHandle pinned = GCHandle.Alloc(data, GCHandleType.Pinned);

            IntPtr address = Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);
            int value = TcpSocketInterop.bind(handle, address, data.Length);

            pinned.Free();

            if (value != 0)
                throw new Exception();
        }

        public void Bind(IPAddress address)
        {
            byte[] data = { 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            GCHandle pinned = GCHandle.Alloc(data, GCHandleType.Pinned);

            byte[] bytes = address.GetAddressBytes();
            Array.Copy(bytes, 0, data, 4, 4);

            IntPtr pointer = Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);
            int value = TcpSocketInterop.bind(handle, pointer, data.Length);

            pinned.Free();

            if (value != 0)
                throw new Exception();
        }

        public TcpSocketInfo Info()
        {
            byte[] data = new byte[128];
            GCHandle pinned = GCHandle.Alloc(data, GCHandleType.Pinned);

            int length = data.Length;
            IntPtr pointer = Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);
            int value = TcpSocketInterop.getsockname(handle, pointer, ref length);

            uint ee = TcpSocketInterop.GetLastError();

            byte[] address = new byte[4];
            Array.Copy(data, 4, address, 0, 4);

            int port = data[2] * 256 + data[3];
            IPEndPoint endpoint = new IPEndPoint(new IPAddress(address), port);

            return new TcpSocketInfo(TcpSocketStatus.OK, this, endpoint);
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

        public void Send(TcpSocketBuffer buffer, TcpSocketSendCallback callback)
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

        public Task<TcpSocketSend> Send(TcpSocketBuffer buffer)
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

        public void Receive(TcpSocketBuffer buffer, TcpSocketReceiveCallback callback)
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

        public Task<TcpSocketReceive> Receive(TcpSocketBuffer buffer)
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
            int value = TcpSocketInterop.closesocket(handle);

            if (value != 0)
                throw new Exception();
        }
    }
}