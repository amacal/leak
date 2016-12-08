namespace Leak.Common
{
    public interface NetworkConnection
    {
        long Identifier { get; }

        PeerAddress Remote { get; }

        NetworkDirection Direction { get; }

        void Receive(NetworkIncomingMessageHandler handler);

        void Send(NetworkOutgoingMessage message);

        void Terminate();
    }
}