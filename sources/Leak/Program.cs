using Leak.Commands;
using Pargos;

namespace Leak
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            ArgumentCollection arguments = ArgumentFactory.Parse(args);
            string command = arguments.GetString(0);

            switch (command)
            {
                case "download":
                    new DownloadCommand(arguments).Execute();
                    break;

                case "deamon":
                    new DeamonCommand(arguments).Execute();
                    break;
            }
        }
    }
}