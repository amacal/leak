using Leak.Client;

namespace Leak
{
    public interface Reporter
    {
        bool Handle(Notification notification);
    }
}