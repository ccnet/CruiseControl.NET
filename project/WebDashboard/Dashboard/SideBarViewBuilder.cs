using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class SideBarViewBuilder
	{
		private readonly ICruiseRequest request;
		private readonly IUserRequestSpecificSideBarViewBuilder slaveBuilder;

		public SideBarViewBuilder(IUserRequestSpecificSideBarViewBuilder BarUserRequestSpecificSideBarViewBuilder, ICruiseRequest request)
		{
			this.slaveBuilder = BarUserRequestSpecificSideBarViewBuilder;
			this.request = request;
		}

		public HtmlTable Execute()
		{
			HtmlTable table = null;
			string serverName = request.ServerName;
			if (serverName == "")
			{
				table = slaveBuilder.GetFarmSideBar();
			}
			else
			{
				string projectName = request.ProjectName;
				if (projectName == "")
				{
					table = slaveBuilder.GetServerSideBar(serverName);
				}
				else
				{
					string buildName = request.BuildName;
					if (buildName == "")
					{
						table = slaveBuilder.GetProjectSideBar(serverName, projectName);	
					}
					else
					{
						table =  slaveBuilder.GetBuildSideBar(serverName, projectName, buildName);
					}
				}
			}
			table.Width = "100%";
			return table;
		}
	}
}
