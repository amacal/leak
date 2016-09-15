using Leak.Core.Metadata;
using System.IO;

namespace Leak.Core.Repository
{
    public class RepositoryTaskWriteData : RepositoryTask
    {
        private readonly RepositoryBlockData[] data;

        public RepositoryTaskWriteData(RepositoryBlockData[] data)
        {
            this.data = data;
        }

        public void Accept(RepositoryTaskVisitor visitor)
        {
        }

        public void Execute(RepositoryContext context)
        {
            Metainfo metainfo = context.Metainfo;
            string destination = context.Destination;

            int size = metainfo.Properties.PieceSize;
            long position = (long)data[0].Piece * size + data[0].Offset;

            using (RepositoryStream stream = new RepositoryStream(destination, metainfo))
            {
                stream.Seek(position, SeekOrigin.Begin);

                foreach (RepositoryBlockData block in data)
                {
                    block.Write(stream.Write);
                    context.Callback.OnWritten(metainfo.Hash, block.ToBlock());
                }
            }
        }
    }
}