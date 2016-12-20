using Leak.Extensions.Metadata;
using Leak.Tasks;

namespace Leak.Metaget
{
    public class MetagetTaskNext : LeakTask<MetagetContext>
    {
        public void Execute(MetagetContext context)
        {
            if (context.Metamine != null && context.Metafile.IsCompleted() == false)
            {
                context.Glue.ForEachPeer(peer =>
                {
                    MetamineStrategy strategy = MetamineStrategy.Sequential;
                    MetamineBlock[] blocks = context.Metamine.Next(strategy, peer);

                    foreach (MetamineBlock block in blocks)
                    {
                        context.Metamine.Reserve(peer, block);
                        context.Glue.SendMetadataRequest(peer, block.Index);
                    }
                });
            }
        }
    }
}