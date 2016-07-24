namespace Leak.Core.Extensions
{
    public class ExtenderExtension
    {
        private readonly byte id;
        private readonly string name;

        public ExtenderExtension(byte id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public byte Id
        {
            get { return id; }
        }

        public string Name
        {
            get { return name; }
        }
    }
}