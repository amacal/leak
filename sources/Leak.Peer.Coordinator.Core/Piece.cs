using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Peer.Coordinator.Core
{
    public class Piece
    {
        private readonly BlockIndex index;
        private readonly DataBlock data;

        public Piece(BlockIndex index, DataBlock data)
        {
            this.index = index;
            this.data = data;
        }

        public BlockIndex Index
        {
            get { return index; }
        }

        public DataBlock Data
        {
            get { return data; }
        }
    }
}