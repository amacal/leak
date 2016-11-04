using Pargos;

namespace Leak.Echo
{
    public class EchoOptions
    {
        [Parameter, At(0)]
        [Match("server")]
        public EchoServerOptions Server { get; set; }

        [Parameter, At(0)]
        [Match("benchmark")]
        public EchoBenchmarkOptions Benchmark { get; set; }
    }
}