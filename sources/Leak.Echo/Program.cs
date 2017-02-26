using Leak.Completion;
using Leak.Sockets;
using Pargos;

namespace Leak.Echo
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using (CompletionThread worker = new CompletionThread())
            {
                EchoOptions options = Argument.Parse<EchoOptions>(args);
                SocketFactory factory = new SocketFactory(worker);

                worker.Start();

                if (options.Server != null)
                {
                    EchoServerStarter.Start(options.Server, factory);
                }

                if (options.Benchmark != null)
                {
                    EchoBenchmarkStarter.Start(options.Benchmark, factory);
                }
            }
        }
    }
}