using Leak.Common;
using Leak.Events;

namespace Leak.Omnibus.Components
{
    public static class OmnibusExtensions
    {
        public static int GetBlocksInPiece(this Metainfo metainfo)
        {
            return metainfo.Properties.PieceSize / metainfo.Properties.BlockSize;
        }

        public static int GetBlocksInTotal(this Metainfo metainfo)
        {
            return (int)((metainfo.Properties.TotalSize - 1) / metainfo.Properties.BlockSize + 1);
        }

        public static void CallDataChanged(this OmnibusHooks hooks, FileHash hash, int completed)
        {
            hooks.OnDataChanged?.Invoke(new DataChanged
            {
                Hash = hash,
                Completed = completed
            });
        }

        public static void CallDataCompleted(this OmnibusHooks hooks, FileHash hash)
        {
            hooks.OnDataCompleted?.Invoke(new DataCompleted
            {
                Hash = hash
            });
        }

        public static void CallPieceReady(this OmnibusHooks hooks, FileHash hash, int piece)
        {
            hooks.OnPieceReady?.Invoke(new PieceReady
            {
                Hash = hash,
                Piece = piece
            });
        }

        public static void CallPieceCompleted(this OmnibusHooks hooks, FileHash hash, int piece)
        {
            hooks.OnPieceCompleted?.Invoke(new PieceCompleted
            {
                Hash = hash,
                Piece = piece
            });
        }

        public static void CallBlockReserved(this OmnibusHooks hooks, FileHash hash, PeerHash peer, BlockIndex block)
        {
            hooks.OnBlockReserved?.Invoke(new BlockReserved
            {
                Hash = hash,
                Peer = peer,
                Block = block
            });
        }
    }
}