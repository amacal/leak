using Pargos;

namespace Leak.Commands
{
    public class DeamonOptions
    {
        [Parameter, At(1)]
        public string Command { get; set; }

        [Parameter, At(2)]
        public string Directory { get; set; }

        [Parameter, At(3)]
        public string Identifier { get; set; }
    }
}