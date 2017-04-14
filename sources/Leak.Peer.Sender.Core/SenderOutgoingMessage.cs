using Leak.Networking.Core;

namespace Leak.Peer.Sender.Core
{
    public interface SenderOutgoingMessage
    {
        string Type { get; }

        NetworkOutgoingMessage Apply(byte id);

        void Release();
    }
}