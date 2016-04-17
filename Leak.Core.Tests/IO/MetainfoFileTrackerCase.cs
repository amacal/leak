namespace Leak.Core.Tests.IO
{
    public class MetainfoFileTrackerCase
    {
        public string Name { get; set; }

        public byte[] Torrent { get; set; }

        public string[] Trackers { get; set; }

        public override string ToString() => Name;
    }
}