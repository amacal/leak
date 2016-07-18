using Leak.Core.Client;
using Leak.Core.Metadata;

namespace Leak
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            //CommandFactory factory = new CommandFactory();
            //ArgumentCollection arguments = ArgumentFactory.Parse(args);
            //Command command = factory.Create(arguments.GetString(0));

            //command.Execute(arguments);

            const string source = "d:\\debian-8.5.0-amd64-CD-1.iso.torrent";
            const string destination = "d:\\leak";

            PeerClient client = new PeerClient(with =>
            {
                with.Destination = destination;
            });

            client.Start(MetainfoFactory.FromFile(source));
        }
    }
}