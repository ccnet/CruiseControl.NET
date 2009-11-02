using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// An in memory session cache.
    /// </summary>
    [ReflectorType("inMemoryCache")]
    public class InMemorySessionCache
        : SessionCacheBase
    {
        public InMemorySessionCache() : this(new SystemClock())
        {
        }

        public InMemorySessionCache(IClock clock) : base(clock)
        {
        }
    }
}
