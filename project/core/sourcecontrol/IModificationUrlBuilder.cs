using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// A builder to convert URLs within modifications into links.
    /// </summary>
    /// <title>IssueUrlBuilder</title>
	[TypeConverter(typeof (ExpandableObjectConverter))]
	public interface IModificationUrlBuilder
	{
		void SetupModification(Modification[] modifications);
	}
}