using System;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// Defines a dynamic value.
    /// </summary>
    public interface IDynamicValue
    {
        /// <summary>
        /// Applies a dynamc value to an object.
        /// </summary>
        /// <param name="value">The object to apply the value to.</param>
        /// <param name="parameters">The parameters to apply.</param>
        void ApplyTo(object value, Dictionary<string, string> parameters);
    }
}
