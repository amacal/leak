using Leak.Core.Common;

namespace Leak.Core.Client.Events
{
    public class FileInitialized
    {
        public FileHash Hash;

        public int Total;

        public int Completed;
    }
}