using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Provides an in-memory cache for sessions.
    /// </summary>
    /// <title>In Memory Security Cache</title>
    /// <version>1.5</version>
    /// <remarks>
    /// <para>
    /// This cache stores sessions in the current instance of CruiseControl.Net. When CruiseControl.Net is restarted the sessions will be lost.
    /// </para>
    /// <para type="warning">
    /// If you have Watch Config File = true (ccservice.exe.config: /configuration/appSettings/add[key='WatchConfigFile'] ), whenever a 
    /// configuration change is detected, the service will be restarted and all sessions will be lost.  If you want sessions to persist across
    /// service restarts, use <link>File Based Security Cache</link>.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;cache type="inMemoryCache" duration="10" mode="sliding"/&gt;
    /// </code>
    /// </example>
    /// <key name="type">
    /// <description>The type of security cache to use.</description>
    /// <value>inMemoryCache</value>
    /// </key>
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
