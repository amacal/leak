namespace Leak.Extensions.Metadata
{
    public class MetadataPlugin : MorePlugin
    {
        public static readonly string Name = "ut_metadata";

        private readonly MetadataHooks hooks;

        public MetadataPlugin(MetadataHooks hooks)
        {
            this.hooks = hooks;
        }

        public MetadataHooks Hooks
        {
            get { return hooks; }
        }

        public void Install(MoreMapping mapping)
        {
            mapping.Add(Name, new MetadataHandler(hooks));
        }
    }
}