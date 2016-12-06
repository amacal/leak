namespace Leak.Core.Leakage
{
    public class LeakPort
    {
        public static readonly LeakPort Nothing = new LeakPort();
        public static readonly LeakPort Random = new LeakPort();

        private readonly int? value;

        private LeakPort()
        {
        }

        public LeakPort(int value)
        {
            this.value = value;
        }

        public int Value
        {
            get { return value.GetValueOrDefault(); }
        }
    }
}