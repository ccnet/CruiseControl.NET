using System.Web.UI;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class SideBarViewBuilder
	{
		private readonly IUserRequestSpecificSideBarViewBuilder slaveBuilder;

		public SideBarViewBuilder(IUserRequestSpecificSideBarViewBuilder BarUserRequestSpecificSideBarViewBuilder)
		{
			this.slaveBuilder = BarUserRequestSpecificSideBarViewBuilder;
		}

		public HtmlTable Execute(ICruiseRequest request)
		{
			string serverName = request.GetServerName();
			if (serverName == "")
			{
				return slaveBuilder.GetFarmSideBar();
			}
			else
			{
				string projectName = request.GetProjectName();
				if (projectName == "")
				{
					return slaveBuilder.GetServerSideBar(serverName);
				}
				else
				{
					string buildName = request.GetBuildName();
					if (buildName == "")
					{
						return slaveBuilder.GetProjectSideBar(serverName, projectName);	
					}
					else
					{
						return slaveBuilder.GetBuildSideBar(serverName, projectName, buildName);
					}
				}
			}
		}
	}
}
