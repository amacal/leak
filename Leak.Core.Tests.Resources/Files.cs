using System;
using System.IO;
using System.Reflection;

namespace Leak.Core.Tests.Resources
{
    public static class Files
    {
        public static readonly byte[] Ubuntu;
        public static readonly byte[] NeuralNetworks;

        static Files()
        {
            Ubuntu = GetFile("ubuntu-15.10-desktop-amd64.iso.torrent");
            NeuralNetworks = GetFile("e046bca3bc837053d1609ef33d623ee5c5af7300.torrent");
        }

        private static byte[] GetFile(string name)
        {
            using (Stream stream = OpenFile(name))
            using (MemoryStream memory = new MemoryStream())
            {
                stream.CopyTo(memory);
                memory.Seek(0, SeekOrigin.Begin);

                return memory.ToArray();
            }
        }

        private static Stream OpenFile(string name)
        {
            Type owner = typeof(Files);
            Assembly assembly = owner.Assembly;

            return assembly.GetManifestResourceStream(owner, $"Data.{name}");
        }
    }
}