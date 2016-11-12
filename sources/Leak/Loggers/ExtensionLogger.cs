namespace Leak.Loggers
{
    public class ExtensionLogger
    {
        public static ExtensionLogger Off()
        {
            return new ExtensionLogger();
        }

        public static ExtensionLogger Normal()
        {
            return new ExtensionLoggerNormal();
        }

        public static ExtensionLogger Verbose()
        {
            return new ExtensionLoggerVerbose();
        }

        public virtual void Handle(string name, object payload)
        {
        }
    }
}