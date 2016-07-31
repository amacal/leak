using Leak.Core.Common;
using Leak.Core.Loop;
using Leak.Core.Messages;
using Leak.Core.Network;

namespace Leak.Core.Collector
{
    public class PeerCollectorChannel
    {
        private readonly PeerCollectorCallback callback;
        private readonly ConnectionLoopChannel inner;

        public PeerCollectorChannel(PeerCollectorCallback callback, ConnectionLoopChannel inner)
        {
            this.callback = callback;
            this.inner = inner;
        }

        public PeerEndpoint Endpoint
        {
            get { return inner.Endpoint; }
        }

        public void Send(KeepAliveMessage message)
        {
            OnOutgoing(message, "keep-alive");
            inner.Send(message);
        }

        public void Send(InterestedMessage message)
        {
            OnOutgoing(message, "interested");
            inner.Send(message);
        }

        public void Send(BitfieldMessage message)
        {
            OnOutgoing(message, "bitfield");
            inner.Send(message);
        }

        public void Send(params RequestOutgoingMessage[] messages)
        {
            inner.Send(messages);
        }

        public void Send(ExtendedOutgoingMessage message)
        {
            OnOutgoing(message, "extended");
            inner.Send(message);
        }

        private void OnOutgoing(NetworkOutgoingMessage message, string type)
        {
            callback.OnOutgoing(inner.Endpoint.Peer, message.ToConnector(type));
        }
    }
}