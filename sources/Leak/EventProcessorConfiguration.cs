using System;

namespace Leak
{
    public interface EventProcessorConfiguration
    {
        Action<dynamic> GetHandler(string type);
    }
}