using Leak.Extensions.Metadata;
using Leak.Extensions.Peers;
using Leak.Listener;
using Leak.Peer.Coordinator;

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

        internal static CoordinatorBuilder WithMetadata(this CoordinatorBuilder builder, SwarmSettings settings, MetadataHooks hooks)
        {
            if (settings.Metadata)
            {
                builder = builder.WithPlugin(new MetadataPlugin(hooks));
            }

            return builder;
        }

        internal static CoordinatorBuilder WithExchange(this CoordinatorBuilder builder, SwarmSettings settings, PeersHooks hooks)
        {
            if (settings.Metadata)
            {
                builder = builder.WithPlugin(new PeersPlugin(hooks));
            }

            return builder;
        }
    }
}