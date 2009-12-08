using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using System.Xml;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// A base class to provide some common functionality for source control providers.
    /// </summary>
    public abstract class SourceControlBase
        : ISourceControl, IParamatisedItem
    {
        #region Private fields
        private IDynamicValue[] myDynamicValues = new IDynamicValue[0];
        #endregion

        #region Public properties
        #region DynamicValues
        /// <summary>
        /// The dynamic values to use for the source control block.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("dynamicValues", Required = false)]
        public IDynamicValue[] DynamicValues
        {
            get { return myDynamicValues; }
            set { myDynamicValues = value; }
        }
        #endregion
        #endregion

        #region Public methods
        #region GetModifications()
        /// <summary>
        /// Get any modifications since the last build.
        /// </summary>
        /// <param name="from">The from result.</param>
        /// <param name="to">The to result.</param>
        /// <returns>The modifications.</returns>
        public abstract Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to);
        #endregion

        #region LabelSourceControl()
        /// <summary>
        /// Label the source code.
        /// </summary>
        /// <param name="result">The result to use for the label.</param>
        public abstract void LabelSourceControl(IIntegrationResult result);
        #endregion

        #region GetSource()
        /// <summary>
        /// Gets the latest source code.
        /// </summary>
        /// <param name="result">The result to use.</param>
        public abstract void GetSource(IIntegrationResult result);
        #endregion

        #region Initialize()
        /// <summary>
        /// Initialise the SCM plugin.
        /// </summary>
        /// <param name="project"></param>
        public abstract void Initialize(IProject project);
        #endregion

        #region Purge()
        /// <summary>
        /// Purge any old source.
        /// </summary>
        /// <param name="project"></param>
        public abstract void Purge(IProject project);
        #endregion

        #region ApplyParameters()
        /// <summary>
        /// Applies the input parameters to the task.
        /// </summary>
        /// <param name="parameters">The parameters to apply.</param>
        /// <param name="parameterDefinitions">The original parameter definitions.</param>
        public virtual void ApplyParameters(Dictionary<string, string> parameters, IEnumerable<ParameterBase> parameterDefinitions)
        {
            if (myDynamicValues != null)
            {
                foreach (IDynamicValue value in myDynamicValues)
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
