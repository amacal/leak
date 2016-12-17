namespace Leak.Glue.Extensions.Metadata
{
    public class MetadataPlugin : GluePlugin
    {
        public static readonly string Name = "ut_metadata";

        private readonly MetadataHooks hooks;

        public MetadataPlugin(MetadataHooks hooks)
        {
            this.hooks = hooks;
        }

        public void Install(GlueMore more)
        {
            more.Add(Name, new MetadataHandler(hooks));
        }
    }
}