using Leak.Core.Common;
using Leak.Core.Messages;
using System;

namespace Leak.Loggers
{
    public class PeerLoggerVerbose : PeerLoggerNormal
    {
        public override void OnBlockReceived(FileHash hash, PeerHash peer, Piece piece)
        {
            Console.WriteLine($"{peer}: block; piece={piece.Index}; offset={piece.Offset}; size={piece.Size}");
        }
    }
}