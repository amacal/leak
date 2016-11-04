using Pargos;

namespace Leak.Echo
{
    public class EchoServerOptions
    {
        [Option("--port")]
        public int? Port { get; set; }
    }
}