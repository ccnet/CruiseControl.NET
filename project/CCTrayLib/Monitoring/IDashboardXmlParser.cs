using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public interface IDashboardXmlParser
	{
		ProjectStatus ExtractAsProjectStatus(string sourceXml, string projectName);
		string[] ExtractProjectNames(string sourceXml);
	}

}