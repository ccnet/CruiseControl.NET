using System.IO;
using System.Xml;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport
{
	public class XmlReportAction : IAction
	{
		public const string ACTION_NAME = "XmlStatusReport";

		private readonly IFarmService farmService;


		public XmlReportAction(IFarmService farmService)
		{
			this.farmService = farmService;
		}

		public IResponse Execute(IRequest request)
		{
			ProjectStatusListAndExceptions allProjectStatus = farmService.GetProjectStatusListAndCaptureExceptions();

			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
			xmlWriter.WriteStartElement("Projects");

			foreach (ProjectStatusOnServer projectStatusOnServer in allProjectStatus.StatusAndServerList)
			{
				WriteBuildStatus(xmlWriter, projectStatusOnServer);
			}

			xmlWriter.WriteEndElement();

			return new HtmlFragmentResponse(stringWriter.ToString());
		}

		private void WriteBuildStatus(XmlTextWriter xmlWriter, ProjectStatusOnServer projectStatusOnServer)
		{
			ProjectStatus status = projectStatusOnServer.ProjectStatus;

			xmlWriter.WriteStartElement("Project");
			xmlWriter.WriteAttributeString("name", status.Name);
			xmlWriter.WriteAttributeString("activity", status.Activity.ToString());
			xmlWriter.WriteAttributeString("lastBuildStatus", status.BuildStatus.ToString());
			xmlWriter.WriteAttributeString("lastBuildLabel", status.LastSuccessfulBuildLabel);
			xmlWriter.WriteAttributeString("lastBuildTime", XmlConvert.ToString(status.LastBuildDate));
			xmlWriter.WriteAttributeString("nextBuildTime", XmlConvert.ToString(status.NextBuildTime));
			xmlWriter.WriteAttributeString("webUrl", status.WebURL);
			xmlWriter.WriteEndElement();
		}
	}
}