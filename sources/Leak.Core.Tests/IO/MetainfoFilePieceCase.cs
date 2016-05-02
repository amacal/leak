namespace Leak.Core.Tests.IO
{
    public class MetainfoFilePieceCase
    {
        public string Name { get; set; }

        public byte[] Torrent { get; set; }

        public long Size { get; set; }

        public int Count { get; set; }

        public override string ToString() => Name;
    }
}