using System.Threading.Tasks;

namespace Leak.Client
{
    public interface Session
    {
        Task<Notification> NextAsync();
    }
}