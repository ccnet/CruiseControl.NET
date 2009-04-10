using System;
using System.Collections.Generic;
using System.Threading;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Config
{
    /// <summary>
    /// The configuration options for the server.
    /// </summary>
    public class ServerConfiguration
    {
        private List<ExtensionConfiguration> extensions = new List<ExtensionConfiguration>();

        /// <summary>
        /// The extensions to load.
        /// </summary>
        public List<ExtensionConfiguration> Extensions
        {
            get { return extensions; }
        }
    }
}
