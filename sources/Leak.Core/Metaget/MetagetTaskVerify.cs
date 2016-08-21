namespace Leak.Core.Metaget
{
    public class MetagetTaskVerify : MetagetTask
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