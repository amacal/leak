namespace Leak.Common
{
    public interface Component
    {
        ComponentStatus Status { get; }

        void Start();

        void Stop();
    }
}