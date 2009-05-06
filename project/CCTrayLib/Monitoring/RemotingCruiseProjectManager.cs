using System;
using ThoughtWorks.CruiseControl.Remote;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using ThoughtWorks.CruiseControl.Remote.Messages;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	/// Allows access to the state of a single cruise control project, over remoting
	/// </summary>
	public class RemotingCruiseProjectManager : ICruiseProjectManager
	{
        private readonly ICruiseServerClient manager;
		private readonly string projectName;

        public RemotingCruiseProjectManager(ICruiseServerClient manager, string projectName)
		{
			this.manager = manager;
			this.projectName = projectName;
		}

        public void ForceBuild(string sessionToken, Dictionary<string, string> parameters)
		{
			try
			{
                BuildIntegrationRequest request = PopulateRequest(new BuildIntegrationRequest(), sessionToken);
                request.BuildValues = NameValuePair.FromDictionary(parameters);
                ValidateResponse(manager.ForceBuild(request));
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
                MessageRequest request = PopulateRequest(new MessageRequest(), sessionToken);
                request.Message = message;
                ValidateResponse(manager.SendMessage(request));
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
				ValidateResponse(manager.AbortBuild(GenerateProjectRequest(sessionToken)));
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
                ProjectRequest request = GenerateProjectRequest(sessionToken);
                ValidateResponse(manager.Stop(request));
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
                ProjectRequest request = GenerateProjectRequest(sessionToken);
                ValidateResponse(manager.Start(request));
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
                ProjectRequest request = GenerateProjectRequest(sessionToken);
                ValidateResponse(manager.CancelPendingRequest(request));
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
            var request = GenerateProjectRequest(null);
            var response = manager.TakeStatusSnapshot(request);
            ValidateResponse(response);
            ProjectStatusSnapshot snapshot = response.Snapshot;
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
            var request = GenerateProjectRequest(null);
            var response = manager.RetrievePackageList(request);
            ValidateResponse(response);
            PackageDetails[] list = response.Packages.ToArray();
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

        /// <summary>
        /// Retrieves any build parameters.
        /// </summary>
        /// <returns></returns>
        public virtual List<ParameterBase> ListBuildParameters()
        {
            return manager.ListBuildParameters(GenerateProjectRequest(null)).Parameters;
        }

        /// <summary>
        /// Generates a project request to send to a remote server.
        /// </summary>
        /// <param name="sessionToken">The sesison token to use (optional).</param>
        /// <returns>The complete request.</returns>
        private ProjectRequest GenerateProjectRequest(string sessionToken)
        {
            ProjectRequest request = PopulateRequest(new ProjectRequest(), sessionToken);
            return request;
        }

        /// <summary>
        /// Populates a request.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="request"></param>
        /// <param name="sessionToken">The sesison token to use (optional).</param>
        /// <returns>The complete request.</returns>
        private TRequest PopulateRequest<TRequest>(TRequest request, string sessionToken)
            where TRequest : ProjectRequest
        {
            request.SessionToken = sessionToken;
            request.ProjectName = projectName;
            return request;
        }

        /// <summary>
        /// Validates that the request processed ok.
        /// </summary>
        /// <param name="value">The response to check.</param>
        private void ValidateResponse(Response value)
        {
            if (value.Result == ResponseResult.Failure)
            {
                string message = "Request request has failed on the remote server:" + Environment.NewLine +
                    value.ConcatenateErrors();
                throw new CruiseControlException(message);
            }
        }
	}
}
