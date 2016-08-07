using Leak.Core.Omnibus;
using System.Collections.Generic;

namespace Leak.Core.Retriever
{
    public static class ResourceExtensions
    {
        public static OmnibusConfiguration ToOmnibus(this ResourceStorageConfiguration configuration)
        {
            return new OmnibusConfiguration
            {
                Pieces = configuration.Pieces,
                PieceSize = configuration.PieceSize,
                BlockSize = configuration.BlockSize,
                TotalSize = configuration.TotalSize
            };
        }

        public static OmnibusBlock ToOmnibus(this ResourceBlock block)
        {
            return new OmnibusBlock(block.Index, block.Offset, block.Size);
        }

        public static ResourceBlock[] FromOmnibus(this IEnumerable<OmnibusBlock> blocks)
        {
            List<ResourceBlock> result = new List<ResourceBlock>();

            foreach (OmnibusBlock block in blocks)
            {
                result.Add(new ResourceBlock(block.Piece, block.Offset, block.Size));
            }

            return result.ToArray();
        }
    }
}