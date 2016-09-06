using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Retriever.Components;
using Leak.Core.Retriever.Tasks;
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
            context.Repository.Start();
            context.Omnibus.Start();

            context.Callback.OnFileStarted(context.Metainfo.Hash);

            context.Queue.Add(new VerifyPieceTask());
            context.Queue.Add(new FindBitfieldsTask());
        }

        public void OnBitfield(PeerHash peer, Bitfield bitfield)
        {
            context.Queue.Add(new HandleBitfieldTask(peer, bitfield));
        }

        public void OnPiece(PeerHash peer, Piece piece)
        {
            context.Queue.Add(new HandlePieceTask(peer, piece));
        }

        private void OnTick()
        {
            if (context.NextSchedule < DateTime.Now)
            {
                context.Queue.Add(new SchedulePeersTask());
            }

            context.Queue.Process(context);
        }
    }
}