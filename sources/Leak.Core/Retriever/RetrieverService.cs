using Leak.Core.Common;
using Leak.Core.Messages;
using System;

namespace Leak.Core.Retriever
{
    public class RetrieverService
    {
        private readonly RetrieverContext context;

        public RetrieverService(Action<RetrieverConfiguration> configurer)
        {
            context = new RetrieverContext(configurer);
        }

        public void Start()
        {
            context.Timer.Start(OnTick);
            context.Callback.OnStarted(context.Metainfo.Hash);

            context.Queue.Add(new RetrieverTaskVerify());
            context.Queue.Add(new RetrieverTaskFind());
        }

        public void OnBitfield(PeerHash peer, Bitfield bitfield)
        {
            context.Queue.Add(new RetrieverTaskBitfield(peer, bitfield));
        }

        public void OnPiece(PeerHash peer, Piece piece)
        {
            context.Queue.Add(new RetrieverTaskPiece(peer, piece));
        }

        private void OnTick()
        {
            context.Queue.Add(new RetrieverTaskNext());
            context.Queue.Process(context);
        }
    }
}