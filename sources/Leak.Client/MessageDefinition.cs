using Leak.Peer.Receiver;

namespace Leak.Client
{
    public class MessageDefinition : ReceiverDefinition
    {
        public string GetName(byte identifier)
        {
            switch (identifier)
            {
                case 0:
                    return "choke";

                case 1:
                    return "unchoke";

                case 2:
                    return "interested";

                case 4:
                    return "have";

                case 5:
                    return "bitfield";

                case 6:
                    return "request";

                case 7:
                    return "piece";

                case 20:
                    return "extended";

                default:
                    return null;
            }
        }
    }
}