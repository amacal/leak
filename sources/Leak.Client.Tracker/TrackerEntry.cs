using System.Threading.Tasks;
using Leak.Common;

namespace Leak.Client.Tracker
{
    public class TrackerEntry
    {
        public FileHash Hash { get; set; }
        public TaskCompletionSource<TrackerAnnounce> Completion { get; set; }
    }
}