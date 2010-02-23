using System;
using ThoughtWorks.CruiseControl.Remote;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	/// Allows access to the state of a single cruise control project, over remoting
	/// </summary>
	public class RemotingCruiseProjectManager : ICruiseProjectManager
	{
        private readonly CruiseServerClientBase manager;
		private readonly string projectName;

        public RemotingCruiseProjectManager(CruiseServerClientBase manager, string projectName)
		{
			this.manager = manager;
			this.projectName = projectName;
		}

        public void ForceBuild(string sessionToken, Dictionary<string, string> parameters, string userName)
		{
			try
			{
                manager.DisplayName = userName;
                manager.SessionToken = sessionToken;
                if (parameters != null)
                {
                    var buildValues = NameValuePair.FromDictionary(parameters);
                    manager.ForceBuild(projectName, buildValues);
                }
                else
                {
                    manager.ForceBuild(projectName);
                }
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
                string message = string.Format("{0} is fixing the build.", Fixer);
                manager.SessionToken = sessionToken;
                manager.SendMessage(projectName, new Message(message, Message.MessageKind.Fixer));
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
                manager.SessionToken = sessionToken;
                manager.AbortBuild(projectName);
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
                manager.SessionToken = sessionToken;
                manager.StopProject(projectName);
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
                manager.SessionToken = sessionToken;
                manager.StartProject(projectName);
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
                manager.SessionToken = sessionToken;
                manager.CancelPendingRequest(projectName);
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
            var snapshot = manager.TakeStatusSnapshot(projectName);
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
            var list = manager.RetrievePackageList(projectName);
            return list.ToArray();
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
            var fileTransfer = manager.RetrieveFileTransfer(projectName, fileName);
            return fileTransfer;
        }
        #endregion

        /// <summary>
        /// Retrieves any build parameters.
        /// </summary>
        /// <returns></returns>
        public virtual List<ParameterBase> ListBuildParameters()
        {
            return manager.ListBuildParameters(projectName);
        }
	}
}
