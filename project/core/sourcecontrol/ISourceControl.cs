using System;
using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Core
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ISourceControl
	{
		// TODO: is it necessary to specify 'to' date -- just want changes after 'from' date
		Modification[] GetModifications(DateTime from, DateTime to);

		void LabelSourceControl(IIntegrationResult result);
		void GetSource(IIntegrationResult result);

		void Initialize(IProject project);
		void Purge(IProject project);
	}
}