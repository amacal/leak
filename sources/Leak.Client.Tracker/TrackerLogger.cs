namespace Leak.Client.Tracker
{
    public interface TrackerLogger
    {
        void Info(string message);

        void Error(string message);
    }
}