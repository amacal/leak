using Leak.Core.Core;
using System;
using System.Net.Sockets;

namespace Leak.Core.Network
{
    public class NetworkPoolSend : LeakTask<NetworkPool>
    {
        private readonly long identifier;
        private readonly Socket socket;
        private readonly byte[] data;

        public NetworkPoolSend(long identifier, Socket socket, byte[] data)
        {
            this.identifier = identifier;
            this.socket = socket;
            this.data = data;
        }

        public void Execute(NetworkPool context)
        {
            ExecutionHandler handler = new ExecutionHandler(context, identifier);
            SocketAsyncEventArgs args = new SocketAsyncEventArgs
            {
                UserToken = this
            };

            try
            {
                args.SetBuffer(data, 0, data.Length);
                args.Completed += handler.OnCompleted;

                if (socket.SendAsync(args) == false)
                {
                    handler.OnCompleted(null, args);
                }
            }
            catch (Exception ex)
            {
                context.OnException(identifier, ex);
                args.Dispose();
            }
        }

        private class ExecutionHandler
        {
            private readonly NetworkPool context;
            private readonly long identifier;

            public ExecutionHandler(NetworkPool context, long identifier)
            {
                this.context = context;
                this.identifier = identifier;
            }

            public void OnCompleted(object target, SocketAsyncEventArgs args)
            {
                if (args.SocketError != SocketError.Success)
                {
                    context.OnDisconnected(identifier);
                }

                args.Dispose();
            }
        }
    }
}