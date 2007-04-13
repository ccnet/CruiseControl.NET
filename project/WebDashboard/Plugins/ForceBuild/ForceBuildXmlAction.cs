using System.IO;
using System.Xml;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport
{
	public class ForceBuildXmlAction : ICruiseAction
	{
		public const string ACTION_NAME = "ForceBuild";

		private readonly IFarmService farmService;

		public ForceBuildXmlAction(IFarmService farmService)
		{
			this.farmService = farmService;
		}

		public IResponse Execute(ICruiseRequest request)
		{
			farmService.ForceBuild(request.ProjectSpecifier);

			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
			xmlWriter.WriteStartElement("ForceBuildResult");
			xmlWriter.WriteString("Build Forced for " + request.ProjectName);
			xmlWriter.WriteEndElement();

			return new XmlFragmentResponse(stringWriter.ToString());
		}
	}
}