using System.Collections;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IHashtableTransformer
	{
		string Transform(Hashtable transformable, string transformerFileName);
	}
}
