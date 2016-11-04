using Pargos;

namespace Leak.Echo
{
    public class EchoBenchmarkOptions
    {
        [Option("--host")]
        public string Host { get; set; }

        [Option("--port")]
        public int? Port { get; set; }

        [Option("--message-size")]
        public int? Size { get; set; }

        [Option("--workers")]
        public int? Workers { get; set; }
    }
}