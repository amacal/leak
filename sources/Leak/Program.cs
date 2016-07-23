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
        public override void OnInitialized(Metainfo metainfo, PeerClientMetainfoSummary summary)
        {
            Console.WriteLine($"{metainfo.Hash}: initialized; completed={summary.Completed}; total={metainfo.Properties.Pieces}");
        }

        public override void OnCompleted(Metainfo metainfo)
        {
            Console.WriteLine($"{metainfo.Hash}: completed");
        }

        public override void OnPeerConnecting(Metainfo metainfo, string endpoint)
        {
            Console.WriteLine($"{metainfo.Hash}: connecting; endpoint={endpoint}");
        }

        public override void OnPeerConnected(Metainfo metainfo, PeerEndpoint endpoint)
        {
            Console.WriteLine($"{endpoint.Peer}: connected; remote={endpoint.Remote}");
        }

        public override void OnPeerDisconnected(Metainfo metainfo, PeerHash peer)
        {
            Console.WriteLine($"{peer}: disconnected");
        }

        public override void OnPeerBitfield(Metainfo metainfo, PeerHash peer, Bitfield bitfield)
        {
            Console.WriteLine($"{peer}: bitfield; total={bitfield.Length}; completed={bitfield.Completed}");
        }

        public override void OnPeerChoked(Metainfo metainfo, PeerHash peer)
        {
            Console.WriteLine($"{peer}: choke");
        }

        public override void OnPeerUnchoked(Metainfo metainfo, PeerHash peer)
        {
            Console.WriteLine($"{peer}: unchoke");
        }

        public override void OnBlockReceived(Metainfo metainfo, PeerHash peer, Piece piece)
        {
            Console.WriteLine($"{peer}: block; piece={piece.Index}; offset={piece.Offset}; size={piece.Size}");
        }

        public override void OnPieceVerified(Metainfo metainfo, PeerClientPieceVerification verification)
        {
            Console.WriteLine($"{metainfo.Hash}: verified; piece={verification.Piece}");
        }
    }
}