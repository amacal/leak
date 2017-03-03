using System;
using Leak.Common;

namespace Leak.Client.Tracker
{
    public abstract class TrackerException : Exception
    {
        private readonly FileHash hash;

        public TrackerException(FileHash hash, string message)
            : base(message)
        {
            this.hash = hash;
        }

        public FileHash Hash
        {
            get { return hash; }
        }
    }
}