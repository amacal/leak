using Leak.Core.Cando.Metadata;
using Leak.Core.Common;

namespace Leak.Core.Metaget
{
    public class MetagetTaskData : MetagetTask
    {
        private readonly PeerHash peer;
        private readonly MetadataData data;

        public MetagetTaskData(PeerHash peer, MetadataData data)
        {
            this.peer = peer;
            this.data = data;
        }

        public void Execute(MetagetContext context)
        {
            if (context.Metamine != null)
            {
                context.Callback.OnMetadataReceived(peer, data.Block);
                context.Metamine.Complete(data.Block, data.Size);
                context.Metafile.Write(data.Block, data.Payload);
            }
        }
    }
}