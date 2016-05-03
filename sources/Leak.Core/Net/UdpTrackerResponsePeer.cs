using System;
using System.Linq;

namespace Leak.Core.Net
{
    public class UdpTrackerResponsePeer : TrackerResponsePeer
    {
        private readonly byte[] data;
        private readonly int offset;

        public UdpTrackerResponsePeer(byte[] data, int offset)
        {
            this.data = data;
            this.offset = offset;
        }

        public override string Host
        {
            get { return String.Join(".", data.Skip(offset).Take(4)); }
        }

        public override int Port
        {
            get { return 256 * data[offset + 4] + data[offset + 5]; }
        }
    }
}