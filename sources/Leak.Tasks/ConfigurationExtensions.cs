using System;

namespace Leak.Tasks
{
    public static class ConfigurationExtensions
    {
        public static TConfiguration Configure<TConfiguration>(this Action<TConfiguration> configurer, Action<TConfiguration> with)
            where TConfiguration : class, new()
        {
            TConfiguration instance = new TConfiguration();

            with.Invoke(instance);
            configurer.Invoke(instance);

            return instance;
        }
    }
}