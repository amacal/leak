using Leak.Common;

namespace Leak.Extensions
{
    public interface MoreHandler
    {
        void OnMessageReceived(FileHash hash, PeerHash peer, byte[] payload);

        void OnMessageSent(FileHash hash, PeerHash peer, byte[] payload);
    }
}