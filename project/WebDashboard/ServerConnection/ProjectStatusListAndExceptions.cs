namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public class ProjectStatusListAndExceptions
	{
		private readonly ProjectStatusOnServer[] statusAndServerList;
		private readonly CruiseServerException[] exceptions;

		public ProjectStatusListAndExceptions(ProjectStatusOnServer[] statusAndServerList, CruiseServerException[] exceptions)
		{
			this.statusAndServerList = statusAndServerList;
			this.exceptions = exceptions;
		}

		public ProjectStatusOnServer[] StatusAndServerList
		{
			get { return this.statusAndServerList; }
		}

		public CruiseServerException[] Exceptions
		{
			get { return this.exceptions; }
		}
	}
}
