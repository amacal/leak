using Leak.Tasks;

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