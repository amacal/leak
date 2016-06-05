namespace Leak.Core.Net
{
    public class PeerExtendedMetadataData
    {
        public int Piece { get; set; }

        public int TotalSize { get; set; }

        public byte[] Data { get; set; }
    }
}