namespace Leak.Core.Omnibus
{
    public static class OmnibusExtensions
    {
        public static int GetBlocksInPiece(this OmnibusConfiguration configuration)
        {
            if (configuration.Pieces == 0)
                return 0;

            return configuration.PieceSize / configuration.BlockSize;
        }

        public static int GetBlocksInTotal(this OmnibusConfiguration configuration)
        {
            if (configuration.Pieces == 0)
                return 0;

            return (int)((configuration.TotalSize - 1) / configuration.BlockSize + 1);
        }
    }
}