using Leak.Core.Common;

namespace Leak.Core.Glue
{
    public interface GlueHandler
    {
        void Handle(FileHash hash, PeerHash peer, byte[] payload);
    }
}