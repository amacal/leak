namespace Leak.Peer.Sender.Tests
{
    public class SenderMessages : SenderDefinition
    {
        public byte? GetIdentifier(string name)
        {
            return name == "found" ? 1 : default(byte?);
        }
    }
}