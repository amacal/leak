namespace Leak.Core.Omnibus
{
    public static class OmnibusExtensions
    {
        public static int GetBlocksInPiece(this OmnibusConfiguration configuration)
        {
            return configuration.Metainfo.Properties.PieceSize / configuration.Metainfo.Properties.BlockSize;
        }

        public static int GetBlocksInTotal(this OmnibusConfiguration configuration)
        {
            return (int)((configuration.Metainfo.Properties.TotalSize - 1) / configuration.Metainfo.Properties.BlockSize + 1);
        }
    }
}