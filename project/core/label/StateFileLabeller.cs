using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Label
{
	[ReflectorType("stateFileLabeller")]
	public class StateFileLabeller 
        : LabellerBase
	{
		private readonly IStateManager stateManager;

		public StateFileLabeller() : this(new FileStateManager(new SystemIoFileSystem()))
		{}

		public StateFileLabeller(IStateManager stateManager)
		{
			this.stateManager = stateManager;
		}

		[ReflectorProperty("project")]
		public string Project;

		public override string Generate(IIntegrationResult integrationResult)
		{
			return stateManager.LoadState(Project).LastSuccessfulIntegrationLabel;
		}
	}
}