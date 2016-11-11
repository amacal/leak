using Leak.Core.Common;

namespace Leak.Core.Client.Events
{
    public class FileChanged
    {
        public FileHash Hash;

        public int Total;

        public int Completed;
    }
}