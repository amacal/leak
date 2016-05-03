using System.Linq;
using System.Text;

namespace Leak.Core.IO
{
    public class TorrentFile
    {
        private readonly MetainfoEntry source;
        private readonly string directory;
        private readonly long offset;

        public TorrentFile(MetainfoEntry source, string directory, long offset)
        {
            this.source = source;
            this.directory = directory;
            this.offset = offset;
        }

        public string Name
        {
            get { return source.Name; }
        }

        public string Path
        {
            get { return GetSafePath(source.Name); }
        }

        public long Offset
        {
            get { return offset; }
        }

        public long Size
        {
            get { return source.Size; }
        }

        private string GetSafePath(string value)
        {
            StringBuilder builder = new StringBuilder();
            char[] invalid = System.IO.Path.GetInvalidFileNameChars();

            foreach (char character in value)
            {
                if (character == System.IO.Path.DirectorySeparatorChar)
                {
                    builder.Append(character);
                }
                else if (invalid.Contains(character) == false)
                {
                    builder.Append(character);
                }
            }

            return System.IO.Path.Combine(directory, builder.ToString());
        }
    }
}