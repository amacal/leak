using Pargos;

namespace Leak
{
    public abstract class Command
    {
        public abstract string Name { get; }

        public abstract void Execute(ArgumentCollection arguments);
    }
}