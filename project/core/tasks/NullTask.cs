using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// <para>
    /// The Null Task is a task that doesn't do anything - it simply returns successfully. This is useful for
    /// projects that simply monitor the source control system for changes but don't need to do anything.
    /// </para>
    /// </summary>
    /// <title>Null Task</title>
    /// <version>1.0</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;nullTask /&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;nullTask simulateFailure="true" /&gt;
    /// </code>
    /// </example>
    [ReflectorType("nullTask")]
    public class NullTask : TaskBase
	{
        /// <summary>
        /// Defines whether to fail the task or not.
        /// </summary>
        /// <version>1.3</version>
        /// <default>false</default>
        /// <remarks>
        /// This can be useful in testing scenarios - but is probably most useful for people developing for
        /// CruiseControl.NET.
        /// </remarks>
        [ReflectorProperty("simulateFailure", Required = false)]
        public bool SimulateFailure = false;

        /// <summary>
        /// The message for the exception. Makes it easier to spot differences between different errors.
        /// </summary>
        /// <version>1.5</version>
        [ReflectorProperty("simulateFailureMessage", Required = false)]
        public string SimulateFailureMessage = "Simulating Failure";


        protected override bool Execute(IIntegrationResult result)
        {
            if (SimulateFailure)
            {
                result.AddTaskResult(SimulateFailureMessage);
                throw new System.Exception(SimulateFailureMessage);
            }
            else
            {
                result.AddTaskResult(string.Empty);
            }
            
            return !SimulateFailure;
        }
    }
}