namespace Leak.Core.Tests.IO
{
    public class MetainfoFileHashCase
    {
        public string Name { get; set; }

        public byte[] Torrent { get; set; }

        public byte[] Hash { get; set; }

        public override string ToString() => Name;
    }
}