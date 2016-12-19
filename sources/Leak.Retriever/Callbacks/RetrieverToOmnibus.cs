namespace Leak.Retriever.Callbacks
{
    //    public override void OnBlockReserved(FileHash hash, OmnibusReservationEvent @event)
    //    {
    //        List<Request> requests = new List<Request>(@event.Count);

    //        foreach (OmnibusBlock block in @event.Blocks)
    //        {
    //            requests.Add(new Request(block.Piece, block.Offset, block.Size));
    //            //context.Collector.Decrease(@event.Peer, 1);
    //        }

    //        //context.Collector.SendPieceRequest(@event.Peer, requests.ToArray());
    //    }

    //    public override void OnBlockExpired(FileHash hash, PeerHash peer, OmnibusBlock block)
    //    {
    //        //context.Collector.Decrease(peer, 20);
    //    }

    //    public override void OnFileCompleted(FileHash hash)
    //    {
    //        context.Repository.Flush();
    //        context.Hooks.CallDataCompleted(hash);
    //    }

    //    public override void OnScheduleRequested(FileHash hash, PeerHash peer)
    //    {
    //        context.Queue.Add(new ScheduleItTask(peer));
    //    }
    //}
}