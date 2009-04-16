using Exortech.NetReflector;
using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// An in memory session cache.
    /// </summary>
    [ReflectorType("inMemoryCache")]
    public class InMemorySessionCache
        : SessionCacheBase
    {
    }
}
