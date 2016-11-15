using Leak.Core.Common;
using Leak.Core.Metadata;
using Leak.Core.Negotiator;
using Leak.Core.Network;

namespace Leak.Core.Glue
{
    public interface GlueService
    {
        FileHash Hash { get; }

        bool Connect(NetworkConnection connection, Handshake handshake);

        bool Disconnect(NetworkConnection connection);

        void AddMetainfo(Metainfo metainfo);
    }
}