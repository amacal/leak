using Leak.Core.Core;

namespace Leak.Core.Metaget
{
    public class MetagetTaskVerify : LeakTask<MetagetContext>
    {
        public void Execute(MetagetContext context)
        {
            if (context.Metafile.IsCompleted() == false)
            {
                context.Metafile.Verify();
            }
        }
    }
}