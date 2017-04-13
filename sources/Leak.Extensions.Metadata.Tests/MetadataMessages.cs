using Leak.Peer.Receiver;

namespace Leak.Extensions.Metadata.Tests
{
    public class MetadataMessages : ReceiverDefinition
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