using Leak.Common;

namespace Leak.Extensions
{
    public interface MoreHandler
    {
        void OnHandshake(FileHash hash, PeerHash peer, byte[] payload);

        void OnMessageReceived(FileHash hash, PeerHash peer, byte[] payload);

        void OnMessageSent(FileHash hash, PeerHash peer, byte[] payload);
    }
}