namespace Leak.Extensions.Peers
{
    public class PeersBuilder
    {
        public PeersPlugin Build()
        {
            return Build(new PeersHooks());
        }

        public PeersPlugin Build(PeersHooks hooks)
        {
            return new PeersPlugin(hooks);
        }
    }
}