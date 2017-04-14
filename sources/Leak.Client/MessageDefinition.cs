using Leak.Peer.Receiver;
using Leak.Peer.Sender;

namespace Leak.Client
{
    public class MessageDefinition : ReceiverDefinition, SenderDefinition
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

        public byte? GetIdentifier(string name)
        {
            switch (name)
            {
                case "choke":
                    return 0;

                case "unchoke":
                    return 1;

                case "interested":
                    return 2;

                case "have":
                    return 4;

                case "bitfield":
                    return 5;

                case "request":
                    return 6;

                case "piece":
                    return 7;

                case "extended":
                    return 20;

                default:
                    return null;
            }
        }
    }
}