using System.Xml.XPath;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	public interface IBuildLogTransformer
	{
		string Transform(XPathDocument document, string xslFile);
		string TransformResultsWithAllStyleSheets(XPathDocument document); 
	}
}
