namespace Leak.Core.Net
{
    public class PlainPeerNegotiator : PeerNegotiator
    {
        private readonly PeerHandshake handshake;

        public PlainPeerNegotiator(PeerHandshake handshake)
        {
            this.handshake = handshake;
        }

        public override void Active(PeerNegotiatorAware channel)
        {
            channel.Send(handshake);
            channel.Receive(message =>
            {
                if (message.Length == 0)
                {
                    channel.Terminate();
                    return;
                }

                if (message.Length < message[0] + 49)
                {
                    channel.Terminate();
                    return;
                }

                channel.Remove(message[0] + 49);
                channel.Continue(new PeerHandshake(message));
            });
        }

        public override void Passive(PeerNegotiatorAware channel)
        {
            channel.Receive(message =>
            {
                if (message.Length == 0)
                {
                    channel.Terminate();
                    return;
                }

                if (message.Length < message[0] + 49)
                {
                    channel.Terminate();
                    return;
                }

                channel.Send(handshake);
                channel.Remove(message[0] + 49);
                channel.Continue(new PeerHandshake(message));
            });
        }
    }
}