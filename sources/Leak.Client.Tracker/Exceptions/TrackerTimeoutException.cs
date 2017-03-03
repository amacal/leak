using Leak.Common;

namespace Leak.Client.Tracker.Exceptions
{
    public sealed class TrackerTimeoutException : TrackerException
    {
        private readonly int seconds;

        public TrackerTimeoutException(FileHash hash, int seconds)
            : base(hash, "The tracker did not respond in the expected period.")
        {
            this.seconds = seconds;
        }

        public int Seconds
        {
            get { return seconds; }
        }
    }
}