using Leak.Commands;
using System.Collections.Generic;
using System.Linq;

namespace Leak
{
    public class CommandFactory
    {
        private readonly List<Command> commands;

        public CommandFactory()
        {
            this.commands = new List<Command>();
            this.commands.Add(new AnalyzeCommand());
            this.commands.Add(new AnnounceCommand());
            this.commands.Add(new DownloadCommand());
        }

        public Command Create(string name)
        {
            return commands.Single(x => x.Name == name);
        }
    }
}