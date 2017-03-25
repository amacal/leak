using System;

namespace Leak.Common
{
    public class Bitfield
    {
        private readonly bool[] items;
        private int completed;

        private int minimum;
        private int maximum;

        public Bitfield(int length)
        {
            this.items = new bool[length];
            this.completed = 0;

            this.minimum = length - 1;
            this.maximum = 0;
        }

        public Bitfield(int length, Bitfield other)
        {
            this.items = new bool[length];
            this.completed = other.completed;

            this.minimum = other.minimum;
            this.maximum = other.maximum;

            Array.Copy(other.items, items, length);
        }

        public int Length
        {
            get { return items.Length; }
        }

        public int Completed
        {
            get { return completed; }
        }

        public int Minimum
        {
            get { return minimum; }
        }

        public int Maximum
        {
            get { return maximum; }
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
                    {
                        completed++;

                        if (index > maximum)
                        {
                            maximum = index;
                        }

                        if (index < minimum)
                        {
                            minimum = index;
                        }
                    }
                    else
                    {
                        completed--;
                    }
                }
            }
        }

        public void Clear()
        {
            completed = 0;
            maximum = 0;

            minimum = items.Length - 1;
            Array.Clear(items, 0, items.Length);
        }

        public override int GetHashCode()
        {
            return items.Length;
        }

        public override bool Equals(object obj)
        {
            Bitfield other = obj as Bitfield;

            return other != null && Equals(this, other);
        }

        private static bool Equals(Bitfield left, Bitfield right)
        {
            if (left.Length != right.Length)
                return false;

            if (left.completed != right.completed)
                return false;

            for (int i = 0; i < left.Length; i++)
                if (left[i] != right[i])
                    return false;

            return true;
        }

        public static Bitfield Random(int length)
        {
            byte[] random = Bytes.Random(length);
            Bitfield result = new Bitfield(length);

            for (int i = 0; i < length; i++)
            {
                result[i] = random[i] % 2 == 0;
            }

            return result;
        }

        public static Bitfield Complete(int length)
        {
            Bitfield result = new Bitfield(length);

            for (int i = 0; i < length; i++)
            {
                result[i] = true;
            }

            return result;
        }

        public static Bitfield Sequence(params bool[] data)
        {
            Bitfield result = new Bitfield(data.Length);

            for (int i = 0; i < data.Length; i++)
            {
                result[i] = data[i];
            }

            return result;
        }
    }
}