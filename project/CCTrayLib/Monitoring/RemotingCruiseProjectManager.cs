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

        public void ForceBuild(string sessionToken)
		{
			try
			{
				manager.Request(sessionToken, ProjectName, new IntegrationRequest(BuildCondition.ForceBuild, Environment.UserName));
			}
			// Silently ignore exceptions that occur due to connection problems
			catch (System.Net.Sockets.SocketException)
			{
			}
			catch (System.Runtime.Remoting.RemotingException)
			{
			}
		}

		public void FixBuild(string sessionToken, string fixingUserName)
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
                manager.SendMessage(sessionToken, ProjectName, new Message(string.Format("{0} is fixing the build.", Fixer)));
			}
			// Silently ignore exceptions that occur due to connection problems
			catch (System.Net.Sockets.SocketException)
			{
			}
			catch (System.Runtime.Remoting.RemotingException)
			{
			}
		}
		
        public void AbortBuild(string sessionToken)
		{
			try
			{
				manager.AbortBuild(sessionToken, ProjectName, Environment.UserName);
			}
			// Silently ignore exceptions that occur due to connection problems
			catch (System.Net.Sockets.SocketException)
			{
			}
			catch (System.Runtime.Remoting.RemotingException)
			{
			}
		}
		
        public void StopProject(string sessionToken)
		{
			try
			{
                manager.Stop(sessionToken, projectName);
			}
			// Silently ignore exceptions that occur due to connection problems
			catch (System.Net.Sockets.SocketException)
			{
			}
			catch (System.Runtime.Remoting.RemotingException)
			{
			}
		}
		
        public void StartProject(string sessionToken)
		{
			try
			{
                manager.Start(sessionToken, projectName);
			}
			// Silently ignore exceptions that occur due to connection problems
			catch (System.Net.Sockets.SocketException)
			{
			}
			catch (System.Runtime.Remoting.RemotingException)
			{
			}
		}
		
        public void CancelPendingRequest(string sessionToken)
		{
			try
			{
                manager.CancelPendingRequest(sessionToken, ProjectName);
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

        #region RetrieveSnapshot()
        /// <summary>
        /// Retrieves a snapshot of the current build status.
        /// </summary>
        /// <returns>The current build status of the project.</returns>
        public virtual ProjectStatusSnapshot RetrieveSnapshot()
        {
            ProjectStatusSnapshot snapshot = manager.TakeStatusSnapshot(projectName);
            return snapshot;
        }
        #endregion

        #region RetrievePackageList()
        /// <summary>
        /// Retrieves the current list of available packages.
        /// </summary>
        /// <returns></returns>
        public virtual PackageDetails[] RetrievePackageList()
        {
            PackageDetails[] list = manager.RetrievePackageList(projectName);
            return list;
        }
        #endregion

        #region RetrieveFileTransfer()
        /// <summary>
        /// Retrieve a file transfer object.
        /// </summary>
        /// <param name="project">The project to retrieve the file for.</param>
        /// <param name="fileName">The name of the file.</param>
        public virtual IFileTransfer RetrieveFileTransfer(string fileName)
        {
            RemotingFileTransfer fileTransfer = manager.RetrieveFileTransfer(projectName, fileName);
            return fileTransfer;
        }
        #endregion
	}
}
