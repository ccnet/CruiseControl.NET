using System.Text;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.State
{
	// TODO - remove this when we update NetReflector
	[ReflectorType("projectstate")]
	public class ProjectStateManager : IStateManager
	{
		private readonly IFileStateManager slaveStateManager;
		private readonly IProject project;

		public ProjectStateManager()
		{
			// DONT USE THIS (Its only here until we update NetReflector serialization)	
		}

		public ProjectStateManager(IProject project, IFileStateManager slaveStateManager)
		{
			this.project = project;
			this.slaveStateManager = slaveStateManager;
		}

		public bool StateFileExists()
		{
			SetFileName();
			return slaveStateManager.StateFileExists();
		}

		public IntegrationResult LoadState()
		{
			SetFileName();
			return slaveStateManager.LoadState();
		}

		public void SaveState(IntegrationResult result)
		{
			SetFileName();
			slaveStateManager.SaveState(result);
		}

		private void SetFileName()
		{
			StringBuilder strBuilder = new StringBuilder();
			foreach (string token in project.Name.Split(' '))
			{
				strBuilder.Append(token.Substring(0,1).ToUpper());
				if (token.Length > 1)
				{
					strBuilder.Append(token.Substring(1));
				}

			}
			slaveStateManager.Filename = strBuilder.ToString() + ".state";
		}
	}
}