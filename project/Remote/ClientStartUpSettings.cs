using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// Defines the start-up settings for a communications client.
    /// </summary>
    public class ClientStartUpSettings
    {
        #region Public properties
        #region BackwardsCompatable
        /// <summary>
        /// Should the client handle server versions older than 1.5.0.
        /// </summary>
        public bool BackwardsCompatable { get; set; }
        #endregion
        #endregion
    }
}
