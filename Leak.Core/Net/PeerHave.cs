namespace Leak.Core.Net
{
    public class PeerHave
    {
        private readonly int index;

        public PeerHave(int index)
        {
            this.index = index;
        }

        public int Index
        {
            get { return index; }
        }
    }
}