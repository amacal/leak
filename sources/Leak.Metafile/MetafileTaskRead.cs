using Leak.Common;
using Leak.Files;
using Leak.Tasks;

namespace Leak.Meta.Store
{
    public class MetafileTaskRead : LeakTask<MetafileContext>
    {
        private readonly FileHash hash;
        private readonly int piece;
        private readonly FileRead read;

        public MetafileTaskRead(FileHash hash, int piece, FileRead read)
        {
            this.hash = hash;
            this.piece = piece;
            this.read = read;
        }

        public void Execute(MetafileContext context)
        {
            context.Hooks.CallMetafileRead(hash, piece, context.TotalSize, read.Buffer.ToBytes(read.Count));
        }
    }
}