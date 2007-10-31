using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	/// Allows access to the state of a single cruise control project, over remoting
	/// </summary>
	public class RemotingCruiseProjectManager : ICruiseProjectManager
	{
		private readonly ICruiseManager manager;
		private readonly string projectName;

        public RemotingCruiseProjectManager(ICruiseManager manager, string projectName)
		{
			this.manager = manager;
			this.projectName = projectName;
		}

		public void ForceBuild()
		{
			manager.Request(ProjectName, new IntegrationRequest(BuildCondition.ForceBuild, Environment.UserName));
		}

		public void FixBuild(string fixingUserName)
		{
            string Fixer;
            if (fixingUserName.Trim().Length == 0)
            {
                Fixer = Environment.UserName;
            }
            else
            {
                Fixer = fixingUserName;
            }

            manager.SendMessage(ProjectName, new Message(string.Format("{0} is fixing the build.", Fixer)));
		}

		public void CancelPendingRequest()
		{
			manager.CancelPendingRequest(ProjectName);
		}

		public string ProjectName
		{
			get { return projectName; }
		}
	}
}
