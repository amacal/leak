using Pargos;

namespace Leak
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CommandFactory factory = new CommandFactory();
            ArgumentCollection arguments = ArgumentFactory.Parse(args);
            Command command = factory.Create(arguments.GetString(0));

            command.Execute(arguments);
        }
    }
}