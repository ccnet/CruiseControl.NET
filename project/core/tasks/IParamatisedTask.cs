using System;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// Marks a task as having input parameters.
    /// </summary>
    public interface IParamatisedTask
    {
        /// <summary>
        /// Applies the input parameters to the task.
        /// </summary>
        /// <param name="parameters">The parameters to apply.</param>
        void ApplyParameters(Dictionary<string, string> parameters);
    }
}
