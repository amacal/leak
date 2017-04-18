using Leak.Common;

namespace Leak.Peer.Sender
{
    public class SenderBuilder
    {
        private readonly SenderConfiguration configuration;

        public SenderBuilder()
        {
            configuration = new SenderConfiguration();
        }

        public SenderBuilder WithHash(FileHash hash)
        {
            return this;
        }

        public SenderBuilder WithDefinition(SenderDefinition definition)
        {
            configuration.Definition = definition;
            return this;
        }

        public SenderService Build()
        {
            return Build(new SenderHooks());
        }

        public SenderService Build(SenderHooks hooks)
        {
            return new SenderService(configuration, hooks);
        }
    }
}