using Leak.Commands;
using Pargos;

namespace Leak
{
    public class Options
    {
        [Parameter, At(0)]
        public string Command { get; set; }

        [Parameter, At(0)]
        [Match("download")]
        public DownloadOptions Download { get; set; }

        [Parameter, At(0)]
        [Match("deamon")]
        public DeamonOptions Deamon { get; set; }
    }
}