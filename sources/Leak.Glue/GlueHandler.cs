using Leak.Common;

namespace Leak.Glue
{
    public interface GlueHandler
    {
        void HandleMessage(FileHash hash, PeerHash peer, byte[] payload);
    }
}