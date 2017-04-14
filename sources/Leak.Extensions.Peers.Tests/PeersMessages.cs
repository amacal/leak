using Leak.Peer.Receiver;
using Leak.Peer.Sender;

namespace Leak.Extensions.Peers.Tests
{
    public class PeersMessages : ReceiverDefinition, SenderDefinition
    {
        public string GetName(byte identifier)
        {
            switch (identifier)
            {
                case 20:
                    return "extended";

                default:
                    return null;
            }
        }

        public byte? GetIdentifier(string name)
        {
            switch (name)
            {
                case "extended":
                    return 20;

                default:
                    return null;
            }
        }
    }
}