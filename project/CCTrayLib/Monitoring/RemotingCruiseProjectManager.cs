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
			try
			{
				manager.Request(ProjectName, new IntegrationRequest(BuildCondition.ForceBuild, Environment.UserName));
			}
			// Silently ignore exceptions that occur due to connection problems
			catch (System.Net.Sockets.SocketException)
			{
			}
			catch (System.Runtime.Remoting.RemotingException)
			{
			}
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

			try
			{
				manager.SendMessage(ProjectName, new Message(string.Format("{0} is fixing the build.", Fixer)));
			}
			// Silently ignore exceptions that occur due to connection problems
			catch (System.Net.Sockets.SocketException)
			{
			}
			catch (System.Runtime.Remoting.RemotingException)
			{
			}
		}
		
		public void AbortBuild()
		{
			try
			{
				manager.AbortBuild(ProjectName, Environment.UserName);
			}
			// Silently ignore exceptions that occur due to connection problems
			catch (System.Net.Sockets.SocketException)
			{
			}
			catch (System.Runtime.Remoting.RemotingException)
			{
			}
		}
		
		public void StopProject()
		{
			try
			{
				manager.Stop(projectName);
			}
			// Silently ignore exceptions that occur due to connection problems
			catch (System.Net.Sockets.SocketException)
			{
			}
			catch (System.Runtime.Remoting.RemotingException)
			{
			}
		}
		
		public void StartProject()
		{
			try
			{
				manager.Start(projectName);
			}
			// Silently ignore exceptions that occur due to connection problems
			catch (System.Net.Sockets.SocketException)
			{
			}
			catch (System.Runtime.Remoting.RemotingException)
			{
			}
		}
		
		public string ProjectIntegratorState
		{
			get { return GetProjectIntegratorStateByProjectName(projectName); }
		}
		
		public void CancelPendingRequest()
		{
			try
			{
				manager.CancelPendingRequest(ProjectName);
			}
			// Silently ignore exceptions that occur due to connection problems
			catch (System.Net.Sockets.SocketException)
			{
			}
			catch (System.Runtime.Remoting.RemotingException)
			{
			}
		}

		public string ProjectName
		{
			get { return projectName; }
		}
		
		private string GetProjectIntegratorStateByProjectName(string projectName)
		{
			string integratorState = string.Empty;
			ProjectStatus[] statusList;
			
			try
			{
				statusList = manager.GetProjectStatus();
			}
			// Silently ignore exceptions that occur due to connection problems
			catch(System.Net.Sockets.SocketException)
			{
				statusList = new ProjectStatus[0];
			}
			catch (System.Runtime.Remoting.RemotingException)
			{
				statusList = new ProjectStatus[0];
			}
			
			foreach(ProjectStatus projectStatus in statusList)
			{
				if (projectStatus.Name.Equals(projectName))
				{
					integratorState = projectStatus.Status.ToString();
				}
			}
			return integratorState;
		}
	}
}
