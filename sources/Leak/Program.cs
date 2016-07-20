using Leak.Core.Client;
using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Metadata;
using System;
using System.Threading;

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
                with.Callback = new Callback();
            });

            client.Start(MetainfoFactory.FromFile(source));
            Thread.Sleep(TimeSpan.FromHours(1));
        }
    }

    public class Callback : PeerClientCallbackBase
    {
        public override void OnPeerConnected(Metainfo metainfo, PeerHash peer)
        {
            Console.WriteLine($"Connected to {peer}");
        }

        public override void OnPeerBitfield(Metainfo metainfo, PeerHash peer, Bitfield bitfield)
        {
            Console.WriteLine($"Bitfield received from {peer}; total={bitfield.Length}; completed={bitfield.Completed}");
        }

        public override void OnPeerUnchoked(Metainfo metainfo, PeerHash peer)
        {
            Console.WriteLine($"Unchoke received from {peer}");
        }

        public override void OnPieceReceived(Metainfo metainfo, PeerHash peer, Piece piece)
        {
            Console.WriteLine($"Piece received from {peer}; index={piece.Index}; offset={piece.Offset}; size={piece.Size}");
        }
    }
}