namespace Leak.Core.Retriever
{
    public class ResourceStorageConfiguration
    {
        public long TotalSize { get; set; }

        public int Pieces { get; set; }

        public int Blocks { get; set; }

        public int BlocksInPiece { get; set; }

        public int BlockSize { get; set; }
    }
}