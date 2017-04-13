namespace Leak.Peer.Receiver.Tests
{
    public class LoopSamples
    {
        public byte[] KeepAlive
        {
            get { return new byte[] { 0x00, 0x00, 0x00, 0x00 }; }
        }

        public byte[] Known
        {
            get { return new byte[] { 0x00, 0x00, 0x00, 0x01, 0x01 }; }
        }

        public byte[] Unknown
        {
            get { return new byte[] { 0x00, 0x00, 0x00, 0x02, 0x02, 0xff }; }
        }
    }
}