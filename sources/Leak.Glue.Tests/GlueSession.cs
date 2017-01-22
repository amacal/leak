using Leak.Common;
using System;

namespace Leak.Glue.Tests
{
    public class GlueSession : IDisposable
    {
        private readonly Metainfo metainfo;
        private readonly GlueSide left;
        private readonly GlueSide right;

        public GlueSession(Metainfo metainfo, GlueSide left, GlueSide right)
        {
            this.metainfo = metainfo;
            this.left = left;
            this.right = right;
        }

        public GlueSide Left
        {
            get { return left; }
        }

        public GlueSide Right
        {
            get { return right; }
        }

        public Metainfo Metainfo
        {
            get { return metainfo; }
        }

        public void Dispose()
        {
            left.Dispose();
            right.Dispose();
        }
    }
}