using Leak.Networking;
using Leak.Tasks;

namespace Leak.Connector
{
    public class PeerConnectorBuilder
    {
        private readonly PeerConnectorDependencies dependencies;
        private readonly PeerConnectorConfiguration configuration;

        public PeerConnectorBuilder()
        {
            dependencies = new PeerConnectorDependencies();
            configuration = new PeerConnectorConfiguration();
        }

        public PeerConnectorBuilder WithPipeline(PipelineService pipeline)
        {
            dependencies.Pipeline = pipeline;
            return this;
        }

        public PeerConnectorBuilder WithNetwork(NetworkPool network)
        {
            dependencies.Network = network;
            return this;
        }

        public PeerConnector Build()
        {
            return Build(new PeerConnectorHooks());
        }

        public PeerConnector Build(PeerConnectorHooks hooks)
        {
            return new PeerConnector(dependencies, hooks, configuration);
        }
    }
}