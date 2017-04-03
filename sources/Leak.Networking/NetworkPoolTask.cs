namespace Leak.Networking
{
    public interface NetworkPoolTask
    {
        bool CanExecute(NetworkPoolQueue queue);

        void Execute(NetworkPoolInstance context, NetworkPoolTaskCallback callback);

        void Block(NetworkPoolQueue queue);

        void Release(NetworkPoolQueue queue);
    }
}