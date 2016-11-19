using F2F.Sandbox;

namespace Leak.Core.Tests.Core
{
    public static class Sandbox
    {
        public static FileSandbox New()
        {
            return new FileSandbox(new EmptyFileLocator());
        }
    }
}