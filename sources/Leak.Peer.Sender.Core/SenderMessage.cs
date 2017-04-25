using Leak.Networking.Core;

namespace Leak.Peer.Sender.Core
{
    public interface SenderMessage
    {
        string Type { get; }

        NetworkOutgoingMessage Apply(byte id);

        void Release();
    }
}