using Leak.Core;
using Leak.Deamon;

namespace Leak.Commands
{
    public class DeamonCommand
    {
        public void Execute(DeamonOptions options)
        {
            LeakDeamon deamon = new LeakDeamon(with =>
            {
                with.Directory = options.Directory;
                with.Identifier = Bytes.Parse(options.Identifier);
            });

            switch (options.Command)
            {
                case "start":
                    deamon.Start();
                    break;

                case "stop":
                    deamon.Stop();
                    break;

                case "serve":
                    deamon.Serve();
                    break;
            }
        }
    }
}