namespace Leak.Core.Tests.IO
{
    public class MetainfoFileNameCase
    {
        public string Name { get; set; }

        public byte[] Torrent { get; set; }

        public string[] Names { get; set; }

        public override string ToString() => Name;
    }
}