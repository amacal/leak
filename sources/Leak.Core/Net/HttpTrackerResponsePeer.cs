using System.Text;

namespace Leak.Core.Net
{
    public class HttpTrackerResponsePeer : TrackerResponsePeer
    {
        private readonly byte[] data;
        private readonly int offset;

        public HttpTrackerResponsePeer(byte[] data, int offset)
        {
            this.data = data;
            this.offset = offset;
        }

        public override string Host
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                builder.Append(data[0 + offset]);
                builder.Append('.');
                builder.Append(data[1 + offset]);
                builder.Append('.');
                builder.Append(data[2 + offset]);
                builder.Append('.');
                builder.Append(data[3 + offset]);

                return builder.ToString();
            }
        }

        public override int Port
        {
            get
            {
                return data[4 + offset] * 256 + data[5 + offset];
            }
        }
    }
}