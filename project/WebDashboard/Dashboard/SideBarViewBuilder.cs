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
					table = slaveBuilder.GetServerSideBar(request.ServerSpecifier);
				}
				else
				{
					string buildName = request.BuildName;
					if (buildName == "")
					{
						table = slaveBuilder.GetProjectSideBar(request.ProjectSpecifier);	
					}
					else
					{
						table =  slaveBuilder.GetBuildSideBar(request.BuildSpecifier);
					}
				}
			}
			table.Width = "100%";
			return table;
		}
	}
}
