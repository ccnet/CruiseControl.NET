using System.IO;
using System.Web;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport
{
	[ReflectorType("viewConfigurationProjectPlugin")]	
	public class ViewConfigurationProjectPlugin : ICruiseAction, IPlugin
	{
		private readonly ICruiseManagerWrapper cruiseManager;

		public ViewConfigurationProjectPlugin(ICruiseManagerWrapper cruiseManager)
		{
			this.cruiseManager = cruiseManager;
		}

		public IResponse Execute(ICruiseRequest cruiseRequest)
		{
			IProjectSpecifier projectSpecifier = cruiseRequest.ProjectSpecifier;
			string projectXml = cruiseManager.GetProject(projectSpecifier);
			return new HtmlFragmentResponse("<pre>" + HttpUtility.HtmlEncode(FormatXml(projectXml)) + "</pre>");
		}

		private string FormatXml(string projectXml)
		{
			XmlDocument document = new XmlDocument();
			document.LoadXml(projectXml);
			StringWriter buffer = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(buffer);
			writer.Formatting = Formatting.Indented;
			document.WriteTo(writer);
			return buffer.ToString();
		}

		public string LinkDescription
		{
			get { return "Project Configuration"; }
		}

		public INamedAction[] NamedActions
		{
			get { return new INamedAction[] {new ImmutableNamedAction("ViewProjectConfiguration", this)}; }
		}
	}
}
