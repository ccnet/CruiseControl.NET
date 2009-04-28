using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// The start-up mode to use.
    /// </summary>
    public enum ProjectStartupMode
    {
        /// <summary>
        /// Use the last state of the project.
        /// </summary>
        UseLastState,
        /// <summary>
        /// Use the initial state of the project.
        /// </summary>
        UseInitialState,
    }
}
