namespace Leak.Peer.Receiver
{
    public class ReceiverBuilder
    {
        private readonly ReceiverParameters parameters;
        private readonly ReceiverConfiguration configuration;

        public ReceiverBuilder()
        {
            parameters = new ReceiverParameters();
            configuration = new ReceiverConfiguration();
        }

        public ReceiverBuilder WithDefinition(ReceiverDefinition definition)
        {
            configuration.Definition = definition;
            return this;
        }

        public ReceiverService Build()
        {
            return Build(new ReceiverHooks());
        }

        public ReceiverService Build(ReceiverHooks hooks)
        {
            return new ReceiverService(parameters, configuration, hooks);
        }
    }
}