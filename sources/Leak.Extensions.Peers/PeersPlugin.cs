using Leak.Glue;

namespace Leak.Extensions.Peers
{
    public class PeersPlugin : GluePlugin
    {
        public static readonly string Name = "ut_pex";

        private readonly PeersHooks hooks;

        public PeersPlugin(PeersHooks hooks)
        {
            this.hooks = hooks;
        }

        public void Install(GlueMore more)
        {
            more.Add(Name, new PeersHandler(hooks));
        }
    }
}
