using System.IO;
using System.Text;

namespace Leak.Core
{
    public static class StreamExtensions
    {
        public static void Write(this Stream stream, string text)
        {
            stream.Write(Encoding.ASCII.GetBytes(text));
        }

        public static void Write(this Stream stream, byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }
    }
}