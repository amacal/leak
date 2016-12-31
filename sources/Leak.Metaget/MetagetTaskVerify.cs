using Leak.Tasks;

namespace Leak.Metaget
{
    public class MetagetTaskVerify : LeakTask<MetagetContext>
    {
        public void Execute(MetagetContext context)
        {
            if (context.Dependencies.Metafile.IsCompleted() == false)
            {
                context.Dependencies.Metafile.Verify();
            }
        }
    }
}