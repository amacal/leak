using Leak.Core.Cando.Metadata;
using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Metamine;

namespace Leak.Core.Metaget
{
    public class MetagetTaskSize : LeakTask<MetagetContext>
    {
        private readonly PeerHash peer;
        private readonly MetadataSize size;

        public MetagetTaskSize(PeerHash peer, MetadataSize size)
        {
            this.peer = peer;
            this.size = size;
        }

        public void Execute(MetagetContext context)
        {
            if (context.Metamine == null && context.Metafile.IsCompleted() == false)
            {
                context.Hooks.CallMetafileMeasured(context.Hash, size.Bytes);
                context.Metamine = new MetamineBitfield(with => { with.Size = size.Bytes; });
            }
        }
    }
}