using Leak.Core.Metadata;

namespace Leak.Core.Omnibus.Components
{
    public static class OmnibusExtensions
    {
        public static int GetBlocksInPiece(this Metainfo metainfo)
        {
            return metainfo.Properties.PieceSize / metainfo.Properties.BlockSize;
        }

        public static int GetBlocksInTotal(this Metainfo metainfo)
        {
            return (int)((metainfo.Properties.TotalSize - 1) / metainfo.Properties.BlockSize + 1);
        }
    }
}