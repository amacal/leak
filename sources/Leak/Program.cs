using Leak.Commands;
using Pargos;
using System;

namespace Leak
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Options options = Argument.Parse<Options>(args);
            string command = options.Command;

            switch (command)
            {
                case "download":
                    new DownloadCommand(options.Download).Execute();
                    break;
            }

            Environment.Exit(0);
        }
    }
}