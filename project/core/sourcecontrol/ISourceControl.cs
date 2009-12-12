using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// Defines a source control block.
    /// </summary>
    /// <title>Source Control Blocks</title>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ISourceControl
	{
		Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to);

		void LabelSourceControl(IIntegrationResult result);
		void GetSource(IIntegrationResult result);

		void Initialize(IProject project);
		void Purge(IProject project);
	}
}