namespace Leak.Client.Swarm
{
    public interface SwarmLogger
    {
        void Info(string message);

        void Error(string message);
    }
}