using Leak.Common;

namespace Leak.Metafile
{
    public class MetafileBuilder
    {
        private readonly MetafileParameters parameters;
        private readonly MetafileHooks hooks;

        public MetafileBuilder()
        {
            parameters = new MetafileParameters();
            hooks = new MetafileHooks();
        }

        public MetafileBuilder WithHash(FileHash hash)
        {
            parameters.Hash = hash;
            return this;
        }

        public MetafileBuilder WithDestination(string destination)
        {
            parameters.Destination = destination;
            return this;
        }

        public MetafileService Build()
        {
            return new MetafileService(parameters, hooks);
        }
    }
}
