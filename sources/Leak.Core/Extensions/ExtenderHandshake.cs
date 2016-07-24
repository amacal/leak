namespace Leak.Core.Extensions
{
    public class ExtenderHandshake
    {
        private readonly ExtenderExtensionCollection extensions;

        public ExtenderHandshake(ExtenderExtensionCollection extensions)
        {
            this.extensions = extensions;
        }

        public ExtenderExtensionCollection Extensions
        {
            get { return extensions; }
        }
    }
}