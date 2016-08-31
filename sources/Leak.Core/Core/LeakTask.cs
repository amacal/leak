namespace Leak.Core.Core
{
    public interface LeakTask<in TContext>
    {
        void Execute(TContext context);
    }
}