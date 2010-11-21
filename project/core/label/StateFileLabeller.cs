using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.State;

namespace ThoughtWorks.CruiseControl.Core.Label
{
    /// <summary>
    /// This labeller retrieves the last successful integration label for a project using the project's state file. You can use this labeller
    /// if you have split your build across multiple projects and you want to use a consistent version across all builds.
    /// </summary>
    /// <version>1.0</version>
    /// <title>State File Labeller</title>
    /// <example>
    /// <code>
    /// &lt;labeller type="stateFileLabeller"&gt;
    /// &lt;project&gt;Common&lt;/project&gt;
    /// &lt;/labeller&gt;
    /// </code>
    /// </example>
	[ReflectorType("stateFileLabeller")]
	public class StateFileLabeller 
        : LabellerBase
	{
		private readonly IStateManager stateManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateFileLabeller" /> class.	
        /// </summary>
        /// <remarks></remarks>
		public StateFileLabeller() : this(new FileStateManager())
		{}

        /// <summary>
        /// Initializes a new instance of the <see cref="StateFileLabeller" /> class.	
        /// </summary>
        /// <param name="stateManager">The state manager.</param>
        /// <remarks></remarks>
		public StateFileLabeller(IStateManager stateManager)
		{
			this.stateManager = stateManager;
		}

        /// <summary>
        /// The project to retrieve the label from. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("project")]
        public string Project { get; set; }

        /// <summary>
        /// Generates the specified integration result.	
        /// </summary>
        /// <param name="integrationResult">The integration result.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public override string Generate(IIntegrationResult integrationResult)
		{
			return stateManager.LoadState(Project).LastSuccessfulIntegrationLabel;
		}
	}
}