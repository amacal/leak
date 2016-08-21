using Leak.Core.Metadata;
using System;
using System.IO;

namespace Leak.Core.Repository
{
    public class RepositoryStream : IDisposable
    {
        private readonly string location;
        private readonly Metainfo metainfo;

        private MetainfoEntry current;
        private FileStream inner;
        private long position;
        private long left;

        public RepositoryStream(string location, Metainfo metainfo)
        {
            this.location = location;
            this.metainfo = metainfo;
            this.position = 0;
        }

        public long Position
        {
            get { return position; }
            set { Seek(value, SeekOrigin.Begin); }
        }

        public long Seek(long offset, SeekOrigin origin)
        {
            long index = 0;
            bool found = false;

            foreach (MetainfoEntry entry in metainfo.Entries)
            {
                if (index + entry.Size > offset)
                {
                    string path = entry.GetPath(location);

                    if (current != entry && inner != null)
                    {
                        inner.Flush();
                        inner.Close();
                        inner.Dispose();
                        inner = null;
                    }

                    if (current != entry)
                    {
                        current = entry;
                        inner = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, metainfo.Properties.BlockSize);
                    }

                    left = index + entry.Size - offset;

                    if (inner.Position != offset - index)
                    {
                        inner.Seek(offset - index, SeekOrigin.Begin);
                    }

                    found = true;
                    break;
                }

                index = index + entry.Size;
            }

            if (found == false && inner != null)
            {
                inner.Flush();
                inner.Close();
                inner.Dispose();

                current = null;
                inner = null;
            }

            return this.position = offset;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            int read = 0;

            if (inner != null)
            {
                read = inner.Read(buffer, offset, count);
                position += read;
                left -= read;

                if (read < count)
                {
                    Seek(position, SeekOrigin.Begin);
                    read += Read(buffer, offset + read, count - read);
                }
            }

            return read;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            int available = count;
            int reduced = 0;

            if (available > left)
            {
                available = (int)left;
                reduced = count - available;
            }

            inner.Write(buffer, offset, available);
            left -= available;
            position += available;

            if (reduced > 0)
            {
                Seek(position, SeekOrigin.Begin);
                Write(buffer, offset + available, count - available);
            }
        }

        public void Dispose()
        {
            if (inner != null)
            {
                inner.Flush();
                inner.Close();
                inner.Dispose();
                inner = null;
            }
        }
    }
}