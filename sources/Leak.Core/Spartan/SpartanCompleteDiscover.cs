using Leak.Common;
using Leak.Core.Core;
using Leak.Events;

namespace Leak.Core.Spartan
{
    public class SpartanCompleteDiscover : LeakTask<SpartanContext>
    {
        private readonly MetadataDiscovered data;

        public SpartanCompleteDiscover(MetadataDiscovered data)
        {
            this.data = data;
        }

        public void Execute(SpartanContext context)
        {
            context.Facts.MetaGet.Stop(context.Pipeline);
            context.Facts.MetaGet = null;

            context.Facts.Complete(SpartanTasks.Discover);
            context.Hooks.CallTaskCompleted(context.Glue.Hash, SpartanTasks.Discover);

            context.Hooks.CallMetadataDiscovered(data);
            context.Glue.SetPieces(data.Metainfo.Pieces.Length);
            context.Facts.Metainfo = data.Metainfo;

            context.Queue.Add(new SpartanScheduleNext(context));
        }
    }
}