using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// Defines the allowed start-up modes for a project.
    /// </summary>
    public enum ProjectInitialState
    {
        /// <summary>
        /// The project will be started.
        /// </summary>
        Started,
        /// <summary>
        /// The project will be stopped.
        /// </summary>
        Stopped,
    }
}
