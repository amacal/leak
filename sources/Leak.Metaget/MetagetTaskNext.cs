using Leak.Common;
using Leak.Tasks;

namespace Leak.Meta.Get
{
    public class MetagetTaskNext : LeakTask<MetagetContext>
    {
        public void Execute(MetagetContext context)
        {
            if (context.Metamine != null && context.Dependencies.Metafile.IsCompleted() == false)
            {
                foreach (PeerHash peer in context.Dependencies.Glue.Peers)
                {
                    MetamineStrategy strategy = MetamineStrategy.Sequential;
                    MetamineBlock[] blocks = context.Metamine.Next(strategy, peer);

                    foreach (MetamineBlock block in blocks)
                    {
                        context.Metamine.Reserve(peer, block);
                        context.Dependencies.Glue.SendMetadataRequest(peer, block.Index);
                    }
                }
            }
        }
    }
}