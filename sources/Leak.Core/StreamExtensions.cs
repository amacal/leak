using System.IO;

namespace Leak.Core
{
    public static class StreamExtensions
    {
        public static void Write(this Stream stream, string text)
        {
            stream.Write(System.Text.Encoding.ASCII.GetBytes(text));
        }

        public static void Write(this Stream stream, byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }
    }
}