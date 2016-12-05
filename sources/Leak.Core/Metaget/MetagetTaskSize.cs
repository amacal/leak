using Leak.Common;
using Leak.Core.Core;
using Leak.Core.Metamine;

namespace Leak.Core.Metaget
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
            if (context.Metamine == null && context.Metafile.IsCompleted() == false)
            {
                context.Hooks.CallMetafileMeasured(context.Glue.Hash, size);
                context.Metamine = new MetamineBitfield(with => { with.Size = size; });
            }
        }
    }
}