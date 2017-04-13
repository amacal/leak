using Leak.Peer.Receiver;

namespace Leak.Extensions.Peers.Tests
{
    public class PeersMessages : ReceiverDefinition
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
    }
}