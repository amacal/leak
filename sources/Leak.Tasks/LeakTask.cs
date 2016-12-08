namespace Leak.Tasks
{
    public interface LeakTask<in TContext>
    {
        void Execute(TContext context);
    }
}