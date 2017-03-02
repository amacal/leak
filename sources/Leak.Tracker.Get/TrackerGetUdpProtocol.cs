namespace Leak.Tracker.Get
{
    public static class TrackerGetUdpProtocol
    {
        public static readonly byte[] Id =
        {
            0x00, 0x00, 0x04, 0x17, 0x27, 0x10, 0x19, 0x80
        };

        public static readonly byte[] Connect =
        {
            0x00, 0x00, 0x00, 0x00
        };

        public static readonly byte[] Announce =
        {
            0x00, 0x00, 0x00, 0x01
        };

        public static readonly byte[] Scrape =
        {
            0x00, 0x00, 0x00, 0x02
        };

        public static readonly byte[] Error =
        {
            0x00, 0x00, 0x00, 0x03
        };
    }
}