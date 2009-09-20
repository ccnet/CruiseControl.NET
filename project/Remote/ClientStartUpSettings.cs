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

        #region UseEncryption
        /// <summary>
        /// Should all communications use encryption.
        /// </summary>
        /// <remarks>
        /// This setting is mutually exclusive with BackwardsCompatable.
        /// </remarks>
        public bool UseEncryption { get; set; }
        #endregion

        #region FetchVersionOnStartUp
        /// <summary>
        /// Gets or sets a value indicating whether the version will be fetched on start-up.
        /// </summary>
        public bool FetchVersionOnStartUp { get; set; }
        #endregion
        #endregion
    }
}
