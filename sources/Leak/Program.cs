using Leak.Core.Client;
using Leak.Core.Common;
using Leak.Core.Extensions.Metadata;
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
                with.Extensions.Register("ut_metadata", 1);
            });

            client.Start(MetainfoFactory.FromFile(source));

            Thread.Sleep(TimeSpan.FromHours(1));
        }
    }

    public class Callback : PeerClientCallbackBase
    {
        public override void OnInitialized(FileHash hash, PeerClientMetainfoSummary summary)
        {
            Console.WriteLine($"{hash}: initialized; completed={summary.Completed}; total={summary.Total}");
        }

        public override void OnCompleted(FileHash hash)
        {
            Console.WriteLine($"{hash}: completed");
            Environment.Exit(0);
        }

        public override void OnPeerConnecting(FileHash hash, string endpoint)
        {
            Console.WriteLine($"{hash}: connecting; endpoint={endpoint}");
        }

        public override void OnPeerConnected(FileHash hash, PeerEndpoint endpoint)
        {
            Console.WriteLine($"{endpoint.Peer}: connected; remote={endpoint.Remote}");
        }

        public override void OnPeerDisconnected(FileHash hash, PeerHash peer)
        {
            Console.WriteLine($"{peer}: disconnected");
        }

        public override void OnPeerBitfield(FileHash hash, PeerHash peer, Bitfield bitfield)
        {
            Console.WriteLine($"{peer}: bitfield; total={bitfield.Length}; completed={bitfield.Completed}");
        }

        public override void OnPeerChoked(FileHash hash, PeerHash peer)
        {
            Console.WriteLine($"{peer}: choke");
        }

        public override void OnPeerUnchoked(FileHash hash, PeerHash peer)
        {
            Console.WriteLine($"{peer}: unchoke");
        }

        public override void OnBlockReceived(FileHash hash, PeerHash peer, Piece piece)
        {
            Console.WriteLine($"{peer}: block; piece={piece.Index}; offset={piece.Offset}; size={piece.Size}");
        }

        public override void OnPieceVerified(FileHash hash, PeerClientPieceVerification verification)
        {
            Console.WriteLine($"{hash}: verified; piece={verification.Piece}");
        }

        public override void OnMetadataReceived(FileHash hash, PeerHash peer, MetadataData data)
        {
            Console.WriteLine($"{hash}: metadata; piece={data.Piece}; total={data.Size}");
        }
    }
}