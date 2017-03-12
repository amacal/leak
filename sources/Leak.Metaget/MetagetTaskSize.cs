using Leak.Common;
using Leak.Tasks;

namespace Leak.Meta.Get
{
    public class MetagetTaskSize : LeakTask<MetagetContext>
    {
        private readonly PeerHash peer;
        private readonly int size;

        public MetagetTaskSize(PeerHash peer, int size)
        {
            this.peer = peer;
            this.size = size;
        }

        public void Execute(MetagetContext context)
        {
            if (context.Metamine == null && context.Dependencies.Metafile.IsCompleted() == false)
            {
                context.Hooks.CallMetafileMeasured(context.Parameters.Hash, size);
                context.Metamine = new MetamineBitfield(with => { with.Size = size; });
            }
        }
    }
}