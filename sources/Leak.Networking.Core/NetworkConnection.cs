namespace Leak.Networking.Core
{
    public interface NetworkConnection
    {
        long Identifier { get; }

        NetworkAddress Remote { get; }

        NetworkDirection Direction { get; }

        void Receive(NetworkIncomingMessageHandler handler);

        void Send(NetworkOutgoingMessage message);

        void Terminate();
    }
}