using F2F.Sandbox;

namespace Leak.Core.Tests.Network
{
    public static class Sandbox
    {
        public static FileSandbox New()
        {
            return new FileSandbox(new EmptyFileLocator());
        }
    }
}