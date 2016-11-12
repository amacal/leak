namespace Leak.Loggers
{
    public class ListenerLogger
    {
        public static ListenerLogger Off()
        {
            return new ListenerLogger();
        }

        public static ListenerLogger Normal()
        {
            return new ListenerLoggerNormal();
        }

        public static ListenerLogger Verbose()
        {
            return new ListenerLoggerVerbose();
        }

        public virtual void Handle(string name, object payload)
        {
        }
    }
}