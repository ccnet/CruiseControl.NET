using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// Defines a dynamic value.
    /// </summary>
    /// <title>Dynamic Values</title>
    public interface IDynamicValue
    {
        /// <summary>
        /// Applies a dynamc value to an object.
        /// </summary>
        /// <param name="value">The object to apply the value to.</param>
        /// <param name="parameters">The parameters to apply.</param>
        /// <param name="parameterDefinitions">The original parameter definitions.</param>
        void ApplyTo(object value, Dictionary<string, string> parameters, IEnumerable<ParameterBase> parameterDefinitions);
    }

    /// <summary>
    /// Defines an item that has dynamic values.
    /// </summary>
    /// <title>Dynamic Values Item</title>
    public interface IWithDynamicValuesItem
    {
        /// <summary>
        /// The array of dynamic values for the item
        /// </summary>
        IDynamicValue[] DynamicValues { get; set; }
    }

}
