using System.Text;

namespace Leak.Core.Net
{
    public class TrackerResponsePeer
    {
        private readonly byte[] data;
        private readonly int offset;

        public TrackerResponsePeer(byte[] data, int offset)
        {
            this.data = data;
            this.offset = offset;
        }

        public string Host
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

        public int Port
        {
            get
            {
                return data[4 + offset] * 256 + data[5 + offset];
            }
        }
    }
}