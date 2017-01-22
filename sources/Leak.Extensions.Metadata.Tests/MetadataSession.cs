using Leak.Common;
using System;

namespace Leak.Extensions.Metadata.Tests
{
    public class MetadataSession : IDisposable
    {
        private readonly FileHash hash;
        private readonly MetadataSide left;
        private readonly MetadataSide right;

        public MetadataSession(FileHash hash, MetadataSide left, MetadataSide right)
        {
            this.hash = hash;
            this.left = left;
            this.right = right;
        }

        public MetadataSide Left
        {
            get { return left; }
        }

        public MetadataSide Right
        {
            get { return right; }
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public void Dispose()
        {
            left.Dispose();
            right.Dispose();
        }
    }
}