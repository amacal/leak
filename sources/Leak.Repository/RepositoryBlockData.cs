using System;
using Leak.Common;

namespace Leak.Repository
{
    public class RepositoryBlockData : IDisposable
    {
        private readonly BlockIndex index;
        private readonly DataBlock data;

        public RepositoryBlockData(BlockIndex index, DataBlock data)
        {
            this.index = index;
            this.data = data;
        }

        public BlockIndex Index
        {
            get { return index; }
        }

        public void Write(DataBlockCallback callback)
        {
            data.Write(callback);
        }

        public void Dispose()
        {
            data.Dispose();
        }
    }
}