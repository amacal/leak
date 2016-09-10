using Leak.Commands;
using Pargos.Attributes;

namespace Leak
{
    public class Options
    {
        [Verb("download")]
        public DownloadOptions Download { get; set; }

        [Verb("deamon")]
        public DeamonOptions Deamon { get; set; }
    }
}