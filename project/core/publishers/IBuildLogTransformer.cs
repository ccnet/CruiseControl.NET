using System;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	public interface IBuildLogTransformer
	{
		string Transform(XmlDocument document, string xslFile);
		string TransformResultsWithAllStyleSheets(XmlDocument document); 
	}
}
