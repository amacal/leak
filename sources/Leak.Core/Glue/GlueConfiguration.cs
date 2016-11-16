namespace Leak.Core.Glue
{
    public class GlueConfiguration
    {
        public GlueConfiguration()
        {
            Plugins = new GluePlugin[0];
        }

        public GluePlugin[] Plugins;

        public int? Pieces;
    }
}