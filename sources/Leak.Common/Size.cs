namespace Leak.Common
{
    public class Size
    {
        private readonly long bytes;

        public Size(long bytes)
        {
            this.bytes = bytes;
        }

        public long Bytes
        {
            get { return bytes; }
        }

        public Size Increase(long value)
        {
            return new Size(bytes + value);
        }

        public override string ToString()
        {
            if (bytes < 1L << 10)
                return ToString(SizeUnit.Bytes);

            if (bytes < 1L << 20)
                return ToString(SizeUnit.KiloBytes);

            if (bytes < 1L << 30)
                return ToString(SizeUnit.MegaBytes);

            if (bytes < 1L << 40)
                return ToString(SizeUnit.GigaBytes);

            return ToString(SizeUnit.Bytes);
        }

        public string ToString(SizeUnit unit)
        {
            switch (unit)
            {
                default:
                    return $"{bytes}B";

                case SizeUnit.KiloBytes:
                    return $"{bytes / 1024}kB";

                case SizeUnit.MegaBytes:
                    return $"{bytes / 1024 / 1024}MB";

                case SizeUnit.GigaBytes:
                    return $"{bytes / 1024 / 1024 / 1024}GB";
            }
        }
    }
}