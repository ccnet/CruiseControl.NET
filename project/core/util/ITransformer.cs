using System.Collections;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public interface ITransformer
	{
		string Transform(string input, string transformerFileName, Hashtable xsltArgs);
	}
}
