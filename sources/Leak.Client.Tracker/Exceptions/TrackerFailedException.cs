using Leak.Common;

namespace Leak.Client.Tracker.Exceptions
{
    public sealed class TrackerFailedException : TrackerException
    {
        private readonly string reason;

        public TrackerFailedException(FileHash hash, string reason)
            : base(hash, "The tracker responded with an error.")
        {
            this.reason = reason;
        }

        public string Reason
        {
            get { return reason; }
        }
    }
}