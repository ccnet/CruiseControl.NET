using System.Collections.Generic;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace ThoughtWorks.CruiseControl.Core.Label
{
    /// <summary>
    /// Abstract base class to provide common functionality for the labellers.
    /// </summary>
    public abstract class LabellerBase
        : ILabeller, IParamatisedItem
    {
        #region Public properties
        #region DynamicValues
        /// <summary>
        /// The dynamic values to use for the labeller.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("dynamicValues", Required = false)]
        public IDynamicValue[] DynamicValues { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Generate()
        /// <summary>
        /// Generate a label.
        /// </summary>
        /// <param name="integrationResult"></param>
        /// <returns></returns>
        public abstract string Generate(IIntegrationResult integrationResult);
        #endregion

        #region Run()
        /// <summary>
        /// Runs the labeller.
        /// </summary>
        /// <param name="result"></param>
        public virtual void Run(IIntegrationResult result)
        {
            result.Label = Generate(result);
        }
        #endregion

        #region ApplyParameters()
        /// <summary>
        /// Applies the input parameters to the labeller.
        /// </summary>
        /// <param name="parameters">The parameters to apply.</param>
        /// <param name="parameterDefinitions">The original parameter definitions.</param>
        public virtual void ApplyParameters(Dictionary<string, string> parameters, IEnumerable<ParameterBase> parameterDefinitions)
        {
            if (DynamicValues != null)
            {
                foreach (var value in DynamicValues)
                {
                    value.ApplyTo(this, parameters, parameterDefinitions);
                }
            }
        }
        #endregion

        #region PreprocessParameters()
        /// <summary>
        /// Preprocesses a node prior to loading it via NetReflector.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        [ReflectionPreprocessor]
        public virtual XmlNode PreprocessParameters(XmlNode inputNode)
        {
            return DynamicValueUtility.ConvertXmlToDynamicValues(inputNode);
        }
        #endregion
        #endregion
    }
}
