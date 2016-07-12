namespace Leak.Core.Net
{
    public class PlainPeerNegotiator : PeerNegotiator
    {
        private readonly PeerHandshakePayload handshake;

        public PlainPeerNegotiator(PeerHandshakePayload handshake)
        {
            this.handshake = handshake;
        }

        public void Active(PeerNegotiatorActiveContext context)
        {
            context.Connection.Send(handshake);
            context.Connection.Receive(message =>
            {
                if (message.Length == 0)
                {
                    context.Terminate();
                    return;
                }

                if (message.Length < message[0] + 49)
                {
                    context.Terminate();
                    return;
                }

                message.Acknowledge(message[0] + 49);
                context.Continue(new PeerHandshakePayload(message), context.Connection);
            });
        }

        public void Passive(PeerNegotiatorPassiveContext context)
        {
            context.Connection.Receive(message =>
            {
                if (message.Length == 0)
                {
                    context.Terminate();
                    return;
                }

                if (message.Length < message[0] + 49)
                {
                    context.Terminate();
                    return;
                }

                context.Connection.Send(handshake);
                message.Acknowledge(message[0] + 49);
                context.Continue(new PeerHandshakePayload(message), context.Connection);
            });
        }
    }
}