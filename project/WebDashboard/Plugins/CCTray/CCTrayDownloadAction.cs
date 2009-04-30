using System.IO;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.CCTray
{
	public class CCTrayDownloadAction : ICruiseAction
	{
		public const string ActionName = "CCTrayDownload";
		private readonly IPhysicalApplicationPathProvider physicalApplicationPathProvider;

		public CCTrayDownloadAction(IPhysicalApplicationPathProvider physicalApplicationPathProvider)
		{
			this.physicalApplicationPathProvider = physicalApplicationPathProvider;
		}

		public IResponse Execute(ICruiseRequest cruiseRequest)
		{
			DirectoryInfo cctrayPath = new DirectoryInfo(physicalApplicationPathProvider.GetFullPathFor("cctray"));
			if (cctrayPath.Exists)
			{
				FileInfo[] files = cctrayPath.GetFiles("*CCTray*.*");
				if (files.Length == 1)
				{
					return new RedirectResponse("cctray/" + files[0].Name);
				}
                else if (files.Length > 1)
                {
                    StringBuilder installerList = new StringBuilder();
                    installerList.Append(@"<h3>Multiple CCTray installers available</h3>");
                    installerList.Append(@"<p>Choose one of the following CCTray installers:");
                    installerList.Append(@"<ul>");
                    for (int i = 0; i < files.Length; i++)
                    {
                        installerList.Append(@"<li>");
						installerList.Append(@"<a href=""cctray/");
                        installerList.Append(files[i].Name);
                        installerList.Append(@""">");
                        installerList.Append(files[i].Name);
                        installerList.Append(@"</a>");
                        installerList.Append(@"</li>");
                    }
                    installerList.Append(@"</ul>");
                    installerList.Append(@"</p>");
                    return new HtmlFragmentResponse(installerList.ToString());
                }
			}
			return new HtmlFragmentResponse("<h3>Unable to locate CCTray installer at path: " + cctrayPath + "</h3>");
		}
	}
}