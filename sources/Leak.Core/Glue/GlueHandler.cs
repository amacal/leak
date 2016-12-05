using Leak.Common;

namespace Leak.Core.Glue
{
    public interface GlueHandler
    {
        void HandleMessage(FileHash hash, PeerHash peer, byte[] payload);
    }
}