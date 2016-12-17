using System;

namespace Leak.Glue.Tests
{
    public class GlueSession : IDisposable
    {
        private readonly GlueSide left;
        private readonly GlueSide right;

        public GlueSession(GlueSide left, GlueSide right)
        {
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

        public void Dispose()
        {
            left.Dispose();
            right.Dispose();
        }
    }
}
