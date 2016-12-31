using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Leak.Common;
using Leak.Files;
using Leak.Metadata;
using Leak.Tasks;

namespace Leak.Metafile
{
    public class MetafileTaskVerified : LeakTask<MetafileContext>
    {
        private readonly List<byte[]> data;
        private readonly HashAlgorithm algorithm;
        private readonly FileRead read;

        public MetafileTaskVerified(HashAlgorithm algorithm, FileRead read)
        {
            this.algorithm = algorithm;
            this.read = read;

            data = new List<byte[]>();
        }

        public MetafileTaskVerified(MetafileTaskVerified task, FileRead read)
        {
            this.data = task.data;
            this.algorithm = task.algorithm;

            this.read = read;
        }

        public void Execute(MetafileContext context)
        {
            if (read.Count > 0)
            {
                data.Add(read.Buffer.ToBytes(read.Count));
                algorithm.Push(read.Buffer.Data, read.Buffer.Offset, read.Count);

                read.File.Read(read.Position + read.Count, new FileBuffer(16384), result =>
                {
                    context.Queue.Add(new MetafileTaskVerified(this, result));
                });
            }

            if (read.Count == 0)
            {
                byte[] bytes = algorithm.Complete();
                FileHash computed = new FileHash(bytes);

                if (computed.Equals(context.Parameters.Hash))
                {
                    int position = 0;
                    int size = data.Sum(x => x.Length);

                    bytes = new byte[size];

                    foreach (byte[] chunk in data)
                    {
                        Array.Copy(chunk, 0, bytes, position, chunk.Length);
                        position += chunk.Length;
                    }

                    context.IsCompleted = true;
                    context.TotalSize = size;

                    context.Hooks.CallMetafileVerified(context.Parameters.Hash, MetainfoFactory.FromBytes(bytes), size);
                }
                else
                {
                    context.IsCompleted = false;
                    context.Hooks.CallMetafileRejected(context.Parameters.Hash);
                }
            }
        }
    }
}
