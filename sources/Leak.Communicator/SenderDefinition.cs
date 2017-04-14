namespace Leak.Peer.Sender
{
    public interface SenderDefinition
    {
        byte? GetIdentifier(string name);
    }
}