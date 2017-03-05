using Leak.Tasks;

namespace Leak.Tracker.Get
{
    public class TrackerGetContext
    {
        private readonly TrackerGetParameters parameters;
        private readonly TrackerGetDependencies dependencies;
        private readonly TrackerGetConfiguration configuration;
        private readonly TrackerGetHooks hooks;

        private readonly TrackerGetUdpService udp;
        private readonly TrackerGetHttpService http;

        private readonly TrackerGetCollection collection;
        private readonly LeakQueue<TrackerGetContext> queue;

        public TrackerGetContext(TrackerGetParameters parameters, TrackerGetDependencies dependencies, TrackerGetConfiguration configuration, TrackerGetHooks hooks)
        {
            this.parameters = parameters;
            this.dependencies = dependencies;
            this.configuration = configuration;
            this.hooks = hooks;

            this.udp = new TrackerGetUdpService(this);
            this.http = new TrackerGetHttpService(this);

            this.collection = new TrackerGetCollection();
            this.queue = new LeakQueue<TrackerGetContext>(this);
        }

        public TrackerGetParameters Parameters
        {
            get { return parameters; }
        }

        public TrackerGetDependencies Dependencies
        {
            get { return dependencies; }
        }

        public TrackerGetConfiguration Configuration
        {
            get { return configuration; }
        }

        public TrackerGetHooks Hooks
        {
            get { return hooks; }
        }

        public LeakQueue<TrackerGetContext> Queue
        {
            get { return queue; }
        }

        public TrackerGetCollection Collection
        {
            get { return collection; }
        }

        public TrackerGetUdpService Udp
        {
            get { return udp; }
        }

        public TrackerGetHttpService Http
        {
            get { return http; }
        }
    }
}