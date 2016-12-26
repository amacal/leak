using Leak.Common;
using Leak.Events;
using Leak.Tasks;

namespace Leak.Spartan
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
            context.Facts.MetaGet.Stop();
            context.Facts.MetaGet = null;

            context.Facts.Complete(Goal.Discover);
            context.Hooks.CallTaskCompleted(context.Glue.Hash, Goal.Discover);

            context.Hooks.CallMetadataDiscovered(data);
            context.Glue.SetPieces(data.Metainfo.Pieces.Length);
            context.Facts.Metainfo = data.Metainfo;

            context.Queue.Add(new SpartanScheduleNext(context));
        }
    }
}