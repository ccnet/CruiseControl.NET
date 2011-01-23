using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public interface IDashboardXmlParser
	{
		string[] ExtractProjectNames(string sourceXml);
	    CruiseServerSnapshot ExtractAsCruiseServerSnapshot(string sourceXml);
	}

}