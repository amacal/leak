namespace Leak.Core.Network
{
    public interface NetworkConnection
    {
        long Identifier { get; }

        string Remote { get; }

        NetworkDirection Direction { get; }

        void Receive(NetworkIncomingMessageHandler handler);

        void Send(NetworkOutgoingMessage message);

        void Terminate();
    }
}