namespace Leak.Core.Common
{
    public class Bitfield
    {
        private readonly bool[] items;
        private int completed;

        public Bitfield(int length)
        {
            this.items = new bool[length];
            this.completed = 0;
        }

        public int Length
        {
            get { return items.Length; }
        }

        public int Completed
        {
            get { return completed; }
        }

        public bool this[int index]
        {
            get { return items[index]; }
            set
            {
                if (items[index] != value)
                {
                    items[index] = value;

                    if (value)
                        completed++;
                    else
                        completed--;
                }
            }
        }
    }
}