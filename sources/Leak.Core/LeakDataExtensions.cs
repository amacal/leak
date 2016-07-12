namespace Leak.Core
{
    public static class LeakDataExtensions
    {
        public static void Dispose(this LeakData data)
        {
            data.Listener?.Stop();
            data.Listener?.Dispose();
        }
    }
}