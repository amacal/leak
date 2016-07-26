using Leak.Core;
using Leak.Deamon;
using Pargos;

namespace Leak.Commands
{
    public class DeamonCommand
    {
        private readonly ArgumentCollection arguments;

        public DeamonCommand(ArgumentCollection arguments)
        {
            this.arguments = arguments;
        }

        public void Execute()
        {
            string routine = arguments.GetString(1);
            string directory = arguments.GetString("directory");
            string identifier = arguments.GetString("identifier");

            LeakDeamon deamon = new LeakDeamon(with =>
            {
                with.Directory = directory;
                with.Identifier = Bytes.Parse(identifier);
            });

            switch (routine)
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