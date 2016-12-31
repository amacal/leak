using System.IO;
using System.Security.Cryptography;
using Leak.Common;
using Leak.Metadata;

namespace Leak.Metafile
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
            FileHash hash = context.Parameters.Hash;
            string path = context.Parameters.Destination;

            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite, 16384, FileOptions.None))
            {
                stream.Seek(offset, SeekOrigin.Begin);
                stream.Write(data, 0, data.Length);

                stream.Flush();
                stream.Close();
            }

            context.Hooks.CallMetafileWritten(hash, piece, data.Length);
        }

        public void Verify()
        {
            FileHash hash = context.Parameters.Hash;
            string path = context.Parameters.Destination;

            if (File.Exists(path))
            {
                FileHash computed;

                using (HashAlgorithm algorithm = SHA1.Create())
                using (FileStream stream = File.OpenRead(path))
                {
                    computed = new FileHash(algorithm.ComputeHash(stream));
                }

                if (computed.Equals(hash))
                {
                    byte[] bytes = File.ReadAllBytes(path);
                    Metainfo metainfo = MetainfoFactory.FromBytes(bytes);

                    context.IsCompleted = true;
                    context.Hooks.CallMetafileVerified(hash, metainfo);
                }
                else
                {
                    context.IsCompleted = false;
                    context.Hooks.CallMetafileRejected(hash);
                }
            }
            else
            {
                context.IsCompleted = false;
                context.Hooks.CallMetafileRejected(hash);
            }
        }
    }
}