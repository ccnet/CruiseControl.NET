using System;
using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Core
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ISourceControl : ITask
	{
		// TODO: is it necessary to specify 'to' date -- just want changes after 'from' date
		Modification[] GetModifications(DateTime from, DateTime to);

		void LabelSourceControl(string label, DateTime timeStamp);
		void GetSource(IIntegrationResult result);

		void Initialize(IProject project);
		void Purge(IProject project);
	}
}
