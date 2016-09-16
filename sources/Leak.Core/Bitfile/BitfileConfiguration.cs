using Leak.Core.Common;

namespace Leak.Core.Bitfile
{
    public class BitfileConfiguration
    {
        public FileHash Hash { get; set; }

        public string Destination { get; set; }

        public BitfileCallback Callback { get; set; }
    }
}