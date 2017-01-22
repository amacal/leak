namespace Leak.Extensions.Metadata
{
    public class MetadataBuilder
    {
        public MetadataPlugin Build()
        {
            return Build(new MetadataHooks());
        }

        public MetadataPlugin Build(MetadataHooks hooks)
        {
            return new MetadataPlugin(hooks);
        }
    }
}