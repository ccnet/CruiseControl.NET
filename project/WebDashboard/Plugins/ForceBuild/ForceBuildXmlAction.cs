using System.IO;
using System.Xml;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using System.Collections.Generic;

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
            string sessionToken = null;
            if ((request != null) && (request.Request != null)) request.Request.GetText("sessionToken");
            farmService.ForceBuild(request.ProjectSpecifier, sessionToken);

			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
			xmlWriter.WriteStartElement("ForceBuildResult");
			xmlWriter.WriteString("Build Forced for " + request.ProjectName);
			xmlWriter.WriteEndElement();

			return new XmlFragmentResponse(stringWriter.ToString());
		}
	}
}