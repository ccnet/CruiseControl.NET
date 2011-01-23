using System;

namespace ThoughtWorks.CruiseControl.Remote.Monitor
{
    /// <summary>
    /// Arguments for a build queue change event.
    /// </summary>
    public class BuildQueueChangedArgs
        : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="BuildQueueChangedArgs"/>.
        /// </summary>
        /// <param name="buildQueue">The build queue that changed.</param>
        public BuildQueueChangedArgs(BuildQueue buildQueue)
        {
            BuildQueue = buildQueue;
        }
        #endregion

        #region Public properties
        #region BuildQueue
        /// <summary>
        /// The build queue that has been changed.
        /// </summary>
        public BuildQueue BuildQueue { get; protected set; }
        #endregion
        #endregion
    }
}
