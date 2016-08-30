namespace Leak.Core.Common
{
    public class BitfieldInfo
    {
        private readonly int total;
        private readonly int completed;

        public BitfieldInfo(int total, int completed)
        {
            this.total = total;
            this.completed = completed;
        }

        public int Total
        {
            get { return total; }
        }

        public int Completed
        {
            get { return completed; }
        }
    }
}