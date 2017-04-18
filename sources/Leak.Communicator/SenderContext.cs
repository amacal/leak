namespace Leak.Peer.Sender
{
    public class SenderContext
    {
        private readonly SenderConfiguration configuration;
        private readonly SenderHooks hooks;

        private readonly SenderCollection collection;

        public SenderContext(SenderConfiguration configuration, SenderHooks hooks)
        {
            this.configuration = configuration;
            this.hooks = hooks;
            this.collection = new SenderCollection();
        }

        public SenderCollection Collection
        {
            get { return collection; }
        }

        public SenderConfiguration Configuration
        {
            get { return configuration; }
        }

        public SenderHooks Hooks
        {
            get { return hooks; }
        }
    }
}