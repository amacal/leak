using Leak.Core.Metadata;
using System;
using System.IO;
using System.Linq;

namespace Leak.Core.Repository
{
    public class ResourceRepositoryStream : Stream
    {
        private readonly string location;
        private readonly Metainfo metainfo;

        private MetainfoEntry current;
        private FileStream inner;
        private long position;
        private long left;

        public ResourceRepositoryStream(string location, Metainfo metainfo)
        {
            this.location = location;
            this.metainfo = metainfo;
            this.position = 0;
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { return metainfo.Entries.Sum(x => x.Size); }
        }

        public override long Position
        {
            get { return position; }
            set { Seek(value, SeekOrigin.Begin); }
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long index = 0;

            foreach (MetainfoEntry entry in metainfo.Entries)
            {
                if (index + entry.Size > offset)
                {
                    string path = GetEntryPath(location, entry);

                    current = entry;
                    left = index + entry.Size - offset;
                    inner = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                    inner.Seek(offset - index, SeekOrigin.Begin);

                    break;
                }

                index = index + entry.Size;
            }

            return this.position = offset;
        }

        public override void SetLength(long value)
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return inner.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            inner.Write(buffer, offset, count);
            inner.Flush();
            inner.Close();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            inner.Close();
            inner.Dispose();
        }

        private static string GetEntryPath(string location, MetainfoEntry entry)
        {
            string path = String.Join(Path.DirectorySeparatorChar.ToString(), entry.Name);
            string result = Path.Combine(location, path);

            return result;
        }
    }
}