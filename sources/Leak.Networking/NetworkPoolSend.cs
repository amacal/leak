using System;
using Leak.Common;
using Leak.Sockets;
using Leak.Tasks;

namespace Leak.Networking
{
    public class NetworkPoolSend : LeakTask<NetworkPoolInstance>
    {
        private readonly NetworkPoolListener listener;
        private readonly long identifier;
        private readonly TcpSocket socket;
        private readonly DataBlock block;

        public NetworkPoolSend(NetworkPoolListener listener, long identifier, TcpSocket socket, DataBlock block)
        {
            this.listener = listener;
            this.identifier = identifier;
            this.socket = socket;
            this.block = block;
        }

        public void Execute(NetworkPoolInstance context)
        {
            if (listener.IsAvailable(identifier))
            {
                block.With((data, offset, count) =>
                {
                    socket.Send(new SocketBuffer(data, offset, count), OnSent);
                });
            }
            else
            {
                block.Release();
            }
        }

        private void OnSent(TcpSocketSend sent)
        {
            if (listener.IsAvailable(identifier))
            {
                if (sent.Status != SocketStatus.OK || sent.Count == 0)
                {
                    listener.Disconnect(identifier);
                }

                if (sent.Count > 0)
                {
                    listener.HandleSent(identifier, sent.Count);
                }
            }

            block.Release();
        }
    }
}