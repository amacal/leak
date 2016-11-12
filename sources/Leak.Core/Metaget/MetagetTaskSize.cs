using Leak.Core.Cando.Metadata;
using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Metaget.Events;
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
                context.Metamine = new MetamineBitfield(with =>
                {
                    with.Size = size.Bytes;
                });

                context.Bus.Publish("metadata-measured", new MetadataMeasured
                {
                    Size = size,
                    Hash = context.Configuration.Hash
                });
            }
        }
    }
}