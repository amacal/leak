using Leak.Common;
using Leak.Tasks;

namespace Leak.Metaget
{
    public class MetagetTaskData : LeakTask<MetagetContext>
    {
        private readonly PeerHash peer;
        private readonly int piece;
        private readonly byte[] data;

        public MetagetTaskData(PeerHash peer, int piece, byte[] data)
        {
            this.peer = peer;
            this.piece = piece;
            this.data = data;
        }

        public void Execute(MetagetContext context)
        {
            if (context.Metamine != null && context.Metafile.IsCompleted() == false)
            {
                context.Metamine.Complete(piece, data.Length);
                context.Metafile.Write(piece, data);
            }
        }
    }
}