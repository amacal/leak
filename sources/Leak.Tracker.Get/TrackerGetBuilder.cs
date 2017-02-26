using Leak.Common;
using Leak.Completion;
using Leak.Tasks;

namespace Leak.Tracker.Get
{
    public class TrackerGetBuilder
    {
        private readonly TrackerGetParameters parameters;
        private readonly TrackerGetDependencies dependencies;
        private readonly TrackerGetConfiguration configuration;

        public TrackerGetBuilder()
        {
            parameters = new TrackerGetParameters();
            dependencies = new TrackerGetDependencies();
            configuration = new TrackerGetConfiguration();
        }

        public TrackerGetBuilder WithPipeline(PipelineService pipeline)
        {
            dependencies.Pipeline = pipeline;
            return this;
        }

        public TrackerGetBuilder WithWorker(CompletionWorker worker)
        {
            dependencies.Worker = worker;
            return this;
        }

        public TrackerGetBuilder WithPeer(PeerHash peer)
        {
            configuration.Peer = peer;
            return this;
        }

        public TrackerGetService Build()
        {
            return Build(new TrackerGetHooks());
        }

        public TrackerGetService Build(TrackerGetHooks hooks)
        {
            return new TrackerGetService(parameters, dependencies, configuration, hooks);
        }
    }
}