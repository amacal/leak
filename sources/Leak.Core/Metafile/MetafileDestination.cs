using Leak.Core.Common;
using Leak.Core.Metadata;
using System.IO;
using System.Security.Cryptography;

namespace Leak.Core.Metafile
{
    public class MetafileDestination
    {
        private readonly MetafileContext context;

        public MetafileDestination(MetafileContext context)
        {
            this.context = context;
        }

        public void Write(int piece, byte[] data)
        {
            int offset = piece * 16384;
            string path = context.Path;

            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite, 16384, FileOptions.None))
            {
                stream.Seek(offset, SeekOrigin.Begin);
                stream.Write(data, 0, data.Length);

                stream.Flush();
                stream.Close();
            }

            context.Hooks.CallMetafileWritten(context.Hash, piece, data.Length);
        }

        public void Verify()
        {
            FileHash computed;
            string path = context.Path;

            if (File.Exists(path))
            {
                using (HashAlgorithm algorithm = SHA1.Create())
                using (FileStream stream = File.OpenRead(path))
                {
                    computed = new FileHash(algorithm.ComputeHash(stream));
                }

                if (computed.Equals(context.Hash))
                {
                    byte[] bytes = File.ReadAllBytes(path);
                    Metainfo metainfo = MetainfoFactory.FromBytes(bytes);

                    context.IsCompleted = true;
                    context.Hooks.CallMetafileVerified(context.Hash, metainfo);
                }
                else
                {
                    context.IsCompleted = false;
                    context.Hooks.CallMetafileRejected(context.Hash);
                }
            }
        }
    }
}