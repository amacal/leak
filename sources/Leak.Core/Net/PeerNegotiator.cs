namespace Leak.Core.Net
{
    public class PeerNegotiator
    {
        private readonly PeerHandshake handshake;

        public PeerNegotiator(PeerHandshake handshake)
        {
            this.handshake = handshake;
        }

        public void Active(PeerNegotiatorAware channel)
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

                channel.Handle(new PeerHandshake(message));
                channel.Remove(message);
                channel.Continue();
            });
        }

        public void Passive(PeerNegotiatorAware channel)
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
                channel.Handle(new PeerHandshake(message));
                channel.Remove(message);
                channel.Continue();
            });
        }
    }
}