using Exortech.NetReflector;
using System;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// An abstract base class to add parameters to a task
    /// </summary>
    public abstract class TaskBase
        : IParamatisedTask
    {
        private IDynamicValue[] myDynamicValues = new IDynamicValue[0];

        /// <summary>
        /// The dynamic values to use for the task.
        /// </summary>
        [ReflectorProperty("dynamicValues", Required = false)]
        public IDynamicValue[] DynamicValues
        {
            get { return myDynamicValues; }
            set { myDynamicValues = value;}
        }

        /// <summary>
        /// Applies the input parameters to the task.
        /// </summary>
        /// <param name="parameters">The parameters to apply.</param>
        public virtual void ApplyParameters(Dictionary<string, string> parameters)
        {
            if (myDynamicValues != null)
            {
                foreach (IDynamicValue value in myDynamicValues)
                {
                    value.ApplyTo(this, parameters);
                }
            }
        }
    }
}
