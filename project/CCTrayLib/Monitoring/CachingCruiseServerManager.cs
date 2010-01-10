using System;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class CachingCruiseServerManager : ICruiseServerManager, ICache
	{
		private readonly ICruiseServerManager wrappedManager;
		private CruiseServerSnapshot cachedSnapshot;

		public CachingCruiseServerManager(ICruiseServerManager wrappedManager)
		{
			this.wrappedManager = wrappedManager;
		}

        public BuildServer Configuration
		{
            get { return wrappedManager.Configuration; }
		}

        public string SessionToken
		{
            get { return wrappedManager.SessionToken; }
		}

		public string DisplayName
		{
			get { return wrappedManager.DisplayName; }
		}

		public void CancelPendingRequest(string projectName)
		{
			wrappedManager.CancelPendingRequest(projectName);
		}

		public CruiseServerSnapshot GetCruiseServerSnapshot()
		{
			if (cachedSnapshot == null)
			{
				cachedSnapshot = wrappedManager.GetCruiseServerSnapshot();
			}
			return cachedSnapshot;
		}

        /// <summary>
        /// Gets the list of projects.
        /// </summary>
        /// <returns>The available projects on the server.</returns>
        public CCTrayProject[] GetProjectList()
        {
            return this.wrappedManager.GetProjectList();
        }

		public void InvalidateCache()
		{
			cachedSnapshot = null;
		}

        public bool Login()
        {
            return wrappedManager.Login();
        }

        public void Logout()
        {
            wrappedManager.Logout();
        }
	}
}