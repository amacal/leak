using Leak.Core.Common;

namespace Leak.Core.Client.Events
{
    public class FileInitializedEvent
    {
        private readonly Bitfield bitfield;

        public FileInitializedEvent(Bitfield bitfield)
        {
            this.bitfield = bitfield;
        }

        public int Completed
        {
            get { return bitfield.Completed; }
        }

        public int Total
        {
            get { return bitfield.Length; }
        }
    }
}