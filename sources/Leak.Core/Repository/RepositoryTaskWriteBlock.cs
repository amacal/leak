using Leak.Core.Core;
using Leak.Core.Metadata;
using System.IO;

namespace Leak.Core.Repository
{
    public class RepositoryTaskWriteBlock : LeakTask<RepositoryContext>
    {
        private readonly RepositoryBlockData data;

        public RepositoryTaskWriteBlock(RepositoryBlockData data)
        {
            this.data = data;
        }

        public void Execute(RepositoryContext context)
        {
            Metainfo metainfo = context.Metainfo;
            string destination = context.Destination;

            int size = metainfo.Properties.PieceSize;
            long position = (long)data.Piece * size + data.Offset;

            using (RepositoryStream stream = new RepositoryStream(destination, metainfo))
            {
                stream.Seek(position, SeekOrigin.Begin);
                data.Write(stream.Write);
            }

            context.Callback.OnWritten(metainfo.Hash, data.ToBlock());
        }
    }
}