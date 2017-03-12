using Leak.Common;
using Leak.Tasks;

namespace Leak.Meta.Store
{
    public class MetafileTaskWritten : LeakTask<MetafileContext>
    {
        private readonly FileHash hash;
        private readonly int piece;
        private readonly int length;

        public MetafileTaskWritten(FileHash hash, int piece, int length)
        {
            this.hash = hash;
            this.piece = piece;
            this.length = length;
        }

        public void Execute(MetafileContext context)
        {
            context.Hooks.CallMetafileWritten(hash, piece, length);
            context.Destination.Verify();
        }
    }
}