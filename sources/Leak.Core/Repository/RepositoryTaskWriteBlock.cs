using Leak.Core.Metadata;
using System.Collections.Generic;
using System.IO;

namespace Leak.Core.Repository
{
    public class RepositoryTaskWriteBlock : RepositoryTask, RepositoryTaskMergeable
    {
        private readonly RepositoryBlockData data;

        public RepositoryTaskWriteBlock(RepositoryBlockData data)
        {
            this.data = data;
        }

        public void Accept(RepositoryTaskVisitor visitor)
        {
            visitor.Visit(this);
        }

        public int Piece
        {
            get { return data.Piece; }
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
                data.Dispose();
            }

            context.Callback.OnWritten(metainfo.Hash, data.ToBlock());
        }

        public void MergeInto(ICollection<RepositoryBlockData> blocks)
        {
            blocks.Add(data);
        }
    }
}