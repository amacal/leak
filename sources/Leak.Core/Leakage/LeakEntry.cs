using Leak.Core.Common;
using Leak.Core.Glue;
using Leak.Core.Spartan;

namespace Leak.Core.Leakage
{
    public class LeakEntry
    {
        public FileHash Hash;

        public string Destination;

        public GlueService Glue;

        public SpartanService Spartan;
    }
}