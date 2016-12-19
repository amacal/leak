using System;
using Leak.Common;
using Leak.Events;
using Leak.Omnibus;
using Leak.Repository;

namespace Leak.Retriever.Components
{
    public static class RetrieverExtensions
    {
        public static void Write(this RepositoryService repository, int piece, int offset, DataBlock data)
        {
            repository.Write(new RepositoryBlockData(piece, offset, data));
        }

        public static OmnibusStrategy ToOmnibus(this RetrieverStrategy strategy)
        {
            switch (strategy)
            {
                case RetrieverStrategy.RarestFirst:
                    return OmnibusStrategy.RarestFirst;

                case RetrieverStrategy.Sequential:
                    return OmnibusStrategy.Sequential;
            }

            throw new NotSupportedException();
        }

        public static void CallBlockHandled(this RetrieverHooks hooks, FileHash hash, PeerHash peer, int piece, int block, int size)
        {
            hooks.OnBlockHandled?.Invoke(new BlockHandled
            {
                Hash = hash,
                Peer = peer,
                Piece = piece,
                Block = block,
                Size = size
            });
        }

        public static void CallBlockRequested(this RetrieverHooks hooks, FileHash hash, PeerHash peer, int piece, int block)
        {
            hooks.OnBlockRequested?.Invoke(new BlockRequested
            {
                Hash = hash,
                Peer = peer,
                Piece = piece,
                Block = block
            });
        }

        public static void CallPieceAccepted(this RetrieverHooks hooks, PieceAccepted data)
        {
            hooks.OnPieceAccepted?.Invoke(data);
        }

        public static void CallPieceRejected(this RetrieverHooks hooks, PieceRejected data)
        {
            hooks.OnPieceRejected?.Invoke(data);
        }

        public static void CallDataChanged(this RetrieverHooks hooks, DataChanged data)
        {
            hooks.OnDataChanged?.Invoke(data);
        }

        public static void CallDataCompleted(this RetrieverHooks hooks, DataCompleted data)
        {
            hooks.OnDataCompleted?.Invoke(data);
        }
    }
}