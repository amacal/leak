using System.Linq;

namespace Leak.Core.IO
{
    public class TorrentBlockBitfield
    {
        private readonly TorrentDirectory directory;
        private readonly byte[] data;
        private readonly int blocks;

        public TorrentBlockBitfield(TorrentDirectory directory)
        {
            this.directory = directory;
            this.blocks = this.directory.Pieces.Sum(x => x.Blocks.Count);
            this.data = new byte[(this.blocks - 1) / 8 + 1];
        }

        public bool this[int index]
        {
            get { return (data[index / 8] & (1 << (byte)(7 - (index % 8)))) > 0; }
            set { data[index / 8] |= (byte)(1 << (byte)(7 - (index % 8))); }
        }

        public bool Has(TorrentBlock block)
        {
            int index = GetBlockIndex(block);
            int value = (1 << (byte)(7 - (index % 8)));

            return (data[index / 8] & value) > 0;
        }

        public void Set(TorrentBlock block)
        {
            int index = GetBlockIndex(block);
            int value = (1 << (byte)(7 - (index % 8)));

            data[index / 8] |= (byte)value;
        }

        private int GetBlockIndex(TorrentBlock target)
        {
            int index = 0;

            foreach (TorrentPiece piece in directory.Pieces)
            {
                if (piece.Offset + piece.Size > target.Offset)
                {
                    foreach (TorrentBlock block in piece.Blocks)
                    {
                        if (block.Offset == target.Offset)
                        {
                            break;
                        }

                        index++;
                    }

                    break;
                }

                index += piece.Blocks.Count;
            }

            return index;
        }
    }
}