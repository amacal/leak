using System.IO;
using F2F.Sandbox;

namespace Leak.Core.Tests.Core
{
    public static class SandboxExtensions
    {
        public static string CreateFile(this FileSandbox sandbox, string fileName, byte[] content)
        {
            string path = sandbox.CreateFile(fileName);
            File.WriteAllBytes(path, content);
            return path;
        }
    }
}