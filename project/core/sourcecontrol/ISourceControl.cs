using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Core
{
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