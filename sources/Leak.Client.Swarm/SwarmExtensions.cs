using Leak.Extensions.Metadata;
using Leak.Extensions.Peers;
using Leak.Glue;
using Leak.Listener;

namespace Leak.Client.Swarm
{
    public static class SwarmExtensions
    {
        internal static PeerListenerBuilder WithPort(this PeerListenerBuilder builder, SwarmSettings settings)
        {
            if (settings.ListenerPort != null)
            {
                builder = builder.WithPort(settings.ListenerPort.Value);
            }

            return builder;
        }

        internal static GlueBuilder WithMetadata(this GlueBuilder builder, SwarmSettings settings, MetadataHooks hooks)
        {
            if (settings.Metadata)
            {
                builder = builder.WithPlugin(new MetadataPlugin(hooks));
            }

            return builder;
        }

        internal static GlueBuilder WithExchange(this GlueBuilder builder, SwarmSettings settings, PeersHooks hooks)
        {
            if (settings.Metadata)
            {
                builder = builder.WithPlugin(new PeersPlugin(hooks));
            }

            return builder;
        }
    }
}