namespace Leak.Core.Cando
{
    public static class CandoExtensions
    {
        public static CandoHandler Find(this CandoHandler[] handlers, string extension)
        {
            foreach (CandoHandler handler in handlers)
            {
                if (handler.CanHandle(extension))
                {
                    return handler;
                }
            }

            return null;
        }
    }
}