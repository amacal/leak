using Leak.Common;

namespace Leak.Glue
{
    public interface GlueHandler
    {
        void OnMessageReceived(FileHash hash, PeerHash peer, byte[] payload);

        void OnMessageSent(FileHash hash, PeerHash peer, byte[] payload);
    }
}