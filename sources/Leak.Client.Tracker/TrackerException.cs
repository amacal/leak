using System;

namespace Leak.Client.Tracker
{
    public class TrackerException : Exception
    {
        public TrackerException(string message)
            : base(message)
        {
        }
    }
}