namespace Leak.Networking
{
    public class NetworkPoolConfiguration
    {
        public NetworkPoolConfiguration()
        {
            BufferSize = 32 * 1024;
        }

        public int BufferSize;
    }
}