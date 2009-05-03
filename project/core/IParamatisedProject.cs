using System;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// A project that uses parameters for execution.
    /// </summary>
    public interface IParamatisedProject
    {
        /// <summary>
        /// Perform a prebuild with parameters.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parameterValues"></param>
        void Prebuild(IIntegrationResult result, Dictionary<string, string> parameterValues);

        /// <summary>
        /// Performs a run with parameters.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parameterValues"></param>
        void Run(IIntegrationResult result, Dictionary<string, string> parameterValues);

        /// <summary>
        /// Perform a publish with parameters.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parameterValues"></param>
        void PublishResults(IIntegrationResult result, Dictionary<string, string> parameterValues);

        /// <summary>
        /// Lists the parameters for the project.
        /// </summary>
        /// <returns></returns>
        List<ParameterBase> ListBuildParameters();

        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="parameters"></param>
        void ValidateParameters(Dictionary<string, string> parameters);
    }
}
