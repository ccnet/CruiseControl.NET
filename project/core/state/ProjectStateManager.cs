using System.IO;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.State
{
	public class ProjectStateManager : IStateManager
	{
		private readonly IFileStateManager slaveStateManager;
		private readonly IProject project;

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
			string projectFileName = CalculateProjectFileName();
			RenameOldStyleStateFileIfItExists(projectFileName);
			slaveStateManager.Filename = projectFileName;
		}

		private string CalculateProjectFileName()
		{
			StringBuilder strBuilder = new StringBuilder();
			foreach (string token in project.Name.Split(' '))
			{
				strBuilder.Append(token.Substring(0, 1).ToUpper());
				if (token.Length > 1)
				{
					strBuilder.Append(token.Substring(1));
				}
			}
			return strBuilder.ToString() + ".state";
		}

		// ToDo - take this out when we go 1.0...
		private void RenameOldStyleStateFileIfItExists(string newFileName)
		{
			string oldFilePath = GetPathOfFileInCurrentDirectory("ccnet.state");
			string newFilePath = GetPathOfFileInCurrentDirectory(newFileName);
			if (File.Exists(oldFilePath))
			{
				File.Copy(oldFilePath, newFilePath);
				File.Delete(oldFilePath);
			}
		}

		private string GetPathOfFileInCurrentDirectory(string filename)
		{
			return Path.Combine(Directory.GetCurrentDirectory(), filename);
		}
	}
}